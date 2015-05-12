//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265

//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World;

// Position vector for the light source.
float3 LightSource;
float4 DiffuseColor;
float DiffuseIntensity = 1.0;

//---------------------------------- Input / Output structures ----------------------------------

// Each member of the struct has to be given a "semantic", to indicate what kind of data should go in
// here and how it should be treated. Read more about the POSITION0 and the many other semantics in 
// the MSDN library
struct VertexShaderInput
{
	float4 Position3D : POSITION0;
	float4 Normal : NORMAL0;
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
	float4 Position2D : POSITION0;
	float4 Color: COLOR0;
	float4 Normal : TEXCOORD0;
};

//------------------------------------------ Functions ------------------------------------------

// Implement the Coloring using normals assignment here
float4 NormalColor(float4 normal)
{
	float4 color = float4(normal.x, normal.y, normal.z, 1);
		return float4(color);
}

// Implement the Procedural texturing assignment here
float4 ProceduralColor(float4 normal)
{
	float4 color;
	if (sin(normal.x) > 0)
		//if (((int)(2 * normal.x) + (int)normal.y) & 1 > 0)
		color = NormalColor(normal);
	else
		color = NormalColor(float4(-normal.x, -normal.y, -normal.z, -normal.w));
	return color;
}

//---------------------------------------- Technique: Simple ----------------------------------------

VertexShaderOutput SimpleVertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct.
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform.
	float4 worldPosition = mul(input.Position3D, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position2D = mul(viewPosition, Projection);

	// Relay the input normals.
	output.Normal = input.Normal;

	// Extract the top-left of the world matrix.
	float3x3 rotationAndScale = (float3x3) World;
		float n = mul(input.Normal, rotationAndScale);
	n = normalize(n);
	float lightIntensity = max(0, dot(n, LightSource));
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	return output;
}

float4 SimplePixelShader(VertexShaderOutput input) : COLOR0
{
	//float4 color = ProceduralColor(input.Normal);
	float4 color = saturate(input.Color);

	return color;
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 SimpleVertexShader();
		PixelShader = compile ps_2_0 SimplePixelShader();
	}
}