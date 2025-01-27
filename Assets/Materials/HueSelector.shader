Shader "Unlit/HueSelector"
{
    Properties
    {
        _Center ("Center Point", Vector) = (0.5, 0.5, 0, 0)
        _VoidRadius ("Void Radius", Range(0, 0.5)) = 0.3
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

            // vec3 hsv2rgb(vec3 c)
            // {
            //     vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            //     vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
            //     return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            // }

            // float3 hsv2rgb(float3 c)
            // {
            //     float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            //     float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
            //     return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            // }

            
            v2f vert (float4 pos : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv;
                
                return o;
            }

            float4 _Center;
            float _VoidRadius;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - _Center.xy;

                float dist = length(uv);
                if (dist < _VoidRadius || dist > 0.5)
                {
                    return fixed4(1, 1, 1, 0);
                }

                // return uv.y;
                float angle = atan2(uv.y, uv.x);
                if (angle < 0)
                {
                    angle += 6.283185307179586476925286766559;
                }
                angle = angle / 6.283185307179586476925286766559;
                // return angle;
                
                // float3 rgb = hsv2rgb(float3(angle, 1, 1));
                return fixed4(hsv2rgb(float3(angle, 1, 1)), 1);

                // Hを角度から計算。SとVは1
                // float h = angle;
                // switch (int(h*6))
                // {
                //     case 0: return fixed4(1, h*6.0, 0, 1);
                //     case 1: return fixed4((2.0 - h*6.0), 1, 0, 1);
                //     case 2: return fixed4(0, 1, (h*6.0 - 2.0), 1);
                //     case 3: return fixed4(0, (4.0 - h*6.0), 1, 1);
                //     case 4: return fixed4((h*6.0 - 4.0), 0, 1, 1);
                //     case 5: return fixed4(1, 0, (6.0 - h*6.0), 1);
                // }
                // return fixed4(1, 1, 1, 1);
            }
            ENDCG
        }
    }
}
