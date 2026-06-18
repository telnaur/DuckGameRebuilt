#define PI 3.14

sampler u_sprite;

float2 u_textureRes;
float2 u_frameRes;
float2 u_offset;

float u_time;

bool inRange(float v, float minv, float maxv)
{
    return v > minv && v < maxv;
}

float4 Main(float2 texCoord: TEXCOORD0) : COLOR0
{
    float2 fr = u_frameRes / u_textureRes;
    float2 fc = float2(floor(texCoord.x / fr.x) * fr.x, floor(texCoord.y / fr.y) * fr.y);

    float2 n = (texCoord - fc) / fr - float2(0.5, 0.5);

    float ps = 1 / u_frameRes;
    n -= float2(u_offset.x * ps, u_offset.y * ps);

    float2 pc = float2(floor(n.x / ps), floor(n.y / ps));
    float2 tl = float2(pc.x * ps, pc.y * ps);

    float ang = PI / 4;
    float s = sin(ang);
    float c = cos(ang);
    tl = float2(tl.x * c - tl.y * s, tl.x * s + tl.y * c);

    float b = 0.5 - u_time;

    float4 texCol = tex2D(u_sprite, texCoord);

    float tlb = b - 0.5;
    float brb = -tlb;

    if (!inRange(tl.x, tlb, brb) || !inRange(tl.y, tlb, brb))
    {
        return texCol;
    }

    return texCol * 0.06;
}

technique Tech
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 Main();
    }
}

