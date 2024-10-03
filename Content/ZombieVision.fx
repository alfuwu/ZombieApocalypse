sampler uTex : register(s0);
float uIntensity : register(c0);

float4 PixelShaderFunction(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    float4 col = tex2D(uTex, coords);
    float4 green = float4(0.8, 1.0, 0.4, 1.0);
    return uIntensity > 0 ? col * green * 1.5 : col;
}

technique Technique1
{
    pass ScreenPass
    {
        AlphaBlendEnable = TRUE;
        BlendOp = ADD;
        SrcBlend = SRCALPHA;
        DestBlend = INVSRCALPHA;
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}