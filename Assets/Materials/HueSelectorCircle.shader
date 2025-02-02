Shader "Unlit/HueSelector"
{
    Properties
    {
        _Center ("Center Point", Vector) = (0.5, 0.5, 0, 0)
        _VoidRadius ("Void Radius", Range(0, 0.5)) = 0.3
        _Color ("Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // アルファブレンディング
        ZWrite Off                      // 透明物の深度書き込みを無効化

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "lib/ColorFunction.hlsl"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                // float angle : TEXCOORD0;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert (float4 pos : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv;
                
                return o;
            }

            float4 _Center;
            float _VoidRadius;
            float4 _Color;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - _Center.xy;

                float dist = length(uv);
                if (dist < _VoidRadius || dist > 0.5)
                {
                    return fixed4(1, 1, 1, 0);
                }

                return _Color;
            }
            ENDCG
        }
    }
}
