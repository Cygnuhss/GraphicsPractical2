//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265

//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World;
float4x4 QuadWorld;
// Matrix for inverse transpose of world. Used for correct normals calculations.
float4x4 WorldInverseTranspose;

// Variables for ambient lighting.
float4 AmbientColor;
float AmbientIntensity;

// Variables for diffuse (Lambertian) lighting.
float3 LightSourceDirection;
float4 DiffuseColor;
// This intensity approaches the lighting as in the assignment.
float DiffuseIntensity = 0.5;

// Variables for specular (Blinn-Phong) lighting.
float4 SpecularColor;
float SpecularIntensity;
float SpecularPower;
float3 EyePos;

// Variables for texturing.
bool HasTexture;
texture DiffuseTexture;
sampler2D textureSampler = sampler_state
{
	Texture = (DiffuseTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

// Variables for extra coloring functions.
bool NormalColoring;
bool ProceduralColoring;

//---------------------------------- Input / Output structures ----------------------------------

// Each member of the struct has to be given a "semantic", to indicate what kind of data should go in
// here and how it should be treated. Read more about the POSITION0 and the many other semantics in 
// the MSDN library
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

// The output of the vertex shader. After being passed through the interpolator/rasterizer it is also 
// the input of the pixel shader. 
// Note 1: The values that you pass into this struct in the vertex shader are not the same as what 
// you get as input for the pixel shader. A vertex shader has a single vertex as input, the pixel 
// shader has 3 vertices as input, and lets you determine the color of each pixel in the triangle 
// defined by these three vertices. Therefore, all the values in the struct that you get as input for 
// the pixel shaders have been linearly interpolated between there three vertices!
// Note 2: You cannot use the data with the POSITION0 semantic in the pixel shader.
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color: COLOR0;
	float2 TextureCoordinate: TEXCOORD0;
	float4 Normal : TEXCOORD1;
	// Storing the 3D position in TEXCOORD2, because the POSITION0 semantic cannot be used in the pixel shader.
	float3 WorldPos : TEXCOORD2;
};

//------------------------------------------ Functions ------------------------------------------

float4 NormalColor(float4 normal)
{
	// The output color is based on the normals. Alpha is set to 1.
	float4 color = float4(normal.x, normal.y, normal.z, 1);

	return color;
}

float4 ProceduralColor(float4 normal, float3 position)
{
	float4 color;
	// The width of the stripes is adjustable. A value between 0.05 and 1.0 is recommended.
	// In case of a checkerboard pattern, this is the square size.
	float stripeWidth = 0.25f;
	// Use this test to create a vertical stripe pattern.
	//if (sin((Pi * position.x) / stripeWidth) > 0)
	// Use this test to create a checkerboard pattern.
	if (sin((Pi * position.x) / stripeWidth) > 0
		!= sin((Pi * position.y) / stripeWidth) > 0)
	{
		color = NormalColor(normal);
	}
	else
	{
		color = NormalColor(-normal);
	}

	return color;
}

//---------------------------------------- Technique: Simple ----------------------------------------

VertexShaderOutput SimpleVertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct.
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform.
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	// Relay the input normals.
	output.Normal = input.Normal;
	// Relay the texture coordinates.
	output.TextureCoordinate = input.TextureCoordinate;

	// Use this line for NormalColor and ProceduralColor. Leaving it will not cause harm, as the color
	// will later be overridden by the diffuse color.
	output.Color = input.Normal;
	// Relay the POSITION0 information to the TEXCOORD2 semantic, for use in the pixel shader.
	output.WorldPos = input.Position;

	// Extract the top-left of the world matrix.
	float3x3 rotationAndScale = (float3x3) World;
	float3 normal = mul(input.Normal, rotationAndScale);
	// Use this line instead of the above two to correctly handle the normals with non-uniform scaling.
	//float3 normal = mul(input.Normal, WorldInverseTranspose);
	normal = normalize(normal);
	// The color is proportional to the angle between the surface normal and direction to the light source.
	// Surfaces pointing away from the light do not receive any light.
	float lightIntensity = max(0, dot(normal, -LightSourceDirection));
	// Take the diffuse color and intensity into account.
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	return output;
}

float4 SimplePixelShader(VertexShaderOutput input) : COLOR0
{
	float4 color;

	// Use the normal coloring if this parameter is set.
	if (NormalColoring)
	{
		color = NormalColor(input.Normal);
	}
	// Use the procedural coloring if this parameter is set.
	else if (ProceduralColoring)
	{
		color = ProceduralColor(input.Normal, input.WorldPos);
	}
	// Use the normal lighting otherwise.
	else
	{
		// The ambient color is the same everywhere: a predefined color at a certain intensity.
		float4 ambient = AmbientColor * AmbientIntensity;

		// The light vector l is the direction from the location to the light.
		float3 l = -LightSourceDirection;
		// The normal vector n denotes the normal of the surface.
		float3 n = input.Normal;
		// The view vector v is the vector from the camera to the fragment.
		float3 v = normalize(EyePos - input.WorldPos);
		// Calculate the half vector, which is the bisector of the angle between the view vector v and light vector l.
		float3 h = normalize(v + l);
		float4 specular = SpecularColor * SpecularIntensity * pow(saturate(dot(n, h)), SpecularPower);

		

		// Add the ambient and specular light to the already calculated diffuse light and texture.
		color = saturate(input.Color + ambient + specular);
	}

	return color;
}

VertexShaderOutput QuadVertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct.
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform.
	float4 worldPosition = mul(input.Position, QuadWorld);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);

	// Relay the input normals.
	output.Normal = input.Normal;
	// Relay the texture coordinates.
	output.TextureCoordinate = input.TextureCoordinate;

	// Use this line for NormalColor and ProceduralColor. Leaving it will not cause harm, as the color
	// will later be overridden by the diffuse color.
	output.Color = input.Normal;
	// Relay the POSITION0 information to the TEXCOORD2 semantic, for use in the pixel shader.
	output.WorldPos = input.Position;

	// Extract the top-left of the world matrix.
	float3x3 rotationAndScale = (float3x3) QuadWorld;
		float3 normal = mul(input.Normal, rotationAndScale);
		// Use this line instead of the above two to correctly handle the normals with non-uniform scaling.
		//float3 normal = mul(input.Normal, WorldInverseTranspose);
		normal = normalize(normal);
	// The color is proportional to the angle between the surface normal and direction to the light source.
	// Surfaces pointing away from the light do not receive any light.
	float lightIntensity = max(0, dot(normal, -LightSourceDirection));
	// Take the diffuse color and intensity into account.
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	return output;
}

float4 QuadPixelShader(VertexShaderOutput input) : COLOR0
{
	// Sample the texture colors with no transparency and blend with the diffuse light.
		float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
			textureColor.a = 1;
		input.Color = input.Color * textureColor;
	
	return textureColor;
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 SimpleVertexShader();
		PixelShader = compile ps_2_0 SimplePixelShader();
	}
}

technique Quadshader
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 QuadVertexShader();
		PixelShader = compile ps_2_0 QuadPixelShader();
	}
}
