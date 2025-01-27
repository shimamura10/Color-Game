// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            fixed4 _Color;

            fixed4 frag () : SV_Target
            {
                return fixed4(1, 0, 0, 1);
            }
            ENDCG
        }
    }
}
