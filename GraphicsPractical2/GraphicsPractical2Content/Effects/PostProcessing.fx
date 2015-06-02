//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265

//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// The value for gamma correction.
float Gamma;

// SpriteBatch will set this texture. It is the screen that has to be post-processed.
texture ScreenTexture;

// Sampler for the texture.
sampler TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

//----------------------------------------- Pixel shader ----------------------------------------
float4 GammaPixelShader(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
	// Look up the texture color.
	float4 color = tex2D(TextureSampler, TextureCoordinate);

	// Correct by the given gamma value. The equations are simplified, because the colors range from 0-1.
	float4 outputColor = color;
	outputColor.r = pow(color.r, 1.0 / Gamma);
	outputColor.g = pow(color.g, 1.0 / Gamma);
	outputColor.b = pow(color.b, 1.0 / Gamma);

	return outputColor;
}

//--------------------------------- Technique: Gamma correction ---------------------------------

technique GammaCorrection
{
	pass Pass0
	{
		PixelShader = compile ps_2_0 GammaPixelShader();
	}
}
