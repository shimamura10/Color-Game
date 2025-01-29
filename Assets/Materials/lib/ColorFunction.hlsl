#ifndef COLOR_FUNCTION_INCLUDED
#define COLOR_FUNCTION_INCLUDED

// HSVをRGBに変換する関数
// float3 hsv2rgb(float3 c)
// {
//     float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
//     float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
//     return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
// }

float3 hsv2rgb(float3 c)
{
    float h = c.x;
    float s = c.y;
    float v = c.z;
    float3 p = abs(frac(h + float3(0, 2 / 3.0, 1 / 3.0)) * 6 - 3) - 1;
    return v * lerp(1.0, saturate(p), s);
}

#endif // COLOR_FUNCTION_INCLUDED
