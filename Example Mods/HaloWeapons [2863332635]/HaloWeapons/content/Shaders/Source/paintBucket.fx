sampler u_sprite;

float4 u_color;

float4 Main(float2 texCoord: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    float4 texColor = tex2D(u_sprite, texCoord);

    if (texColor.a <= 0) 
    {
        discard;
    }

    return u_color;
}

technique Tech
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 Main();
    }
}