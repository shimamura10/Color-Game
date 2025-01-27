Shader "Unlit/SelectedColor"
{
    Properties
    {
        _Hue ("Hue", Range(0, 1)) = 0.5
        _Saturation ("Saturation", Range(0, 1)) = 1.0
        _Value ("Value", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "lib/ColorFunction.hlsl"

            float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            float _Hue;
            float _Saturation;
            float _Value;

            fixed4 frag () : SV_Target
            {
                return fixed4(hsv2rgb(float3(_Hue, _Saturation, _Value)), 1);
            }
            ENDCG
        }
    }
}
