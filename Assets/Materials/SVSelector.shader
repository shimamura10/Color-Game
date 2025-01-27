Shader "Unlit/SVSelector"
{
    Properties
    {
        _Hue ("Hue", Range(0, 1)) = 0.0
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
                float2 uv : TEXCOORD0;
            };
            
            v2f vert (float4 pos : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv;
                
                return o;
            }

            float _Hue;
            
            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(hsv2rgb(float3(_Hue, i.uv.x, i.uv.y)), 1);
            }
            ENDCG
        }
    }
}
