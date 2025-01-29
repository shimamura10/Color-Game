// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "StudioMaron/UI-CompositShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

// スクリプトで定義したEnumを取得する
            [Enum(StudioMaron.BlendMode)]_Composition("Composition", float) = 0

// この辺はベースとなったShaderと同じだが、不要なパラメータは[HideInInspector]でInspectorから消している
            [HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
            [HideInInspector]_Stencil("Stencil ID", Float) = 0
            [HideInInspector]_StencilOp("Stencil Operation", Float) = 0
            [HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
            [HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255

            [HideInInspector]_ColorMask("Color Mask", Float) = 15

            [HideInInspector][Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

// GrabPassを使用することを明示
            GrabPass {}

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend One OneMinusSrcAlpha
            ColorMask[_ColorMask]

            Pass
            {
                Name "Default"
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            //****************************************
            //RGB → HSV変換の関数
            //****************************************
            half3 RGBtoHSV(half3 rgb)
            {
                half r = rgb.r;
                half g = rgb.g;
                half b = rgb.b;

                half max = r > g ? r : g;
                max = max > b ? max : b;
                half min = r < g ? r : g;
                min = min < b ? min : b;
                half h = max - min;

                half h_r = (g - b) / h;
                h_r += (h_r < 0.0) ? 6.0 : 0.0;
                half h_g = 2.0 + (b - r) / h;
                half h_b = 4.0 + (r - g) / h;

                h = (h > 0.0) ? ((max == r) ? h_r : ((max == g) ? h_g : h_b)) : h;

                h /= 6.0;
                half s = (max - min);
                s = (max != 0.0) ? s /= max : s;
                half v = max;

                half3 hsv;
                hsv.x = h;
                hsv.y = s;
                hsv.z = v;
                return hsv;
            }

        //****************************************
        //HSV → RGB変換の関数
        //****************************************
        half3 HSVtoRGB(half3 hsv)
        {
            half h = hsv.x;
            half s = hsv.y;
            half v = hsv.z;

            half r = v;
            half g = v;
            half b = v;

            h *= 6.0;
            half f = frac(h);
            switch (floor(h)) {
                default:
                case 0:
                    g *= 1 - s * (1 - f);
                    b *= 1 - s;
                    break;
                case 1:
                    r *= 1 - s * f;
                    b *= 1 - s;
                    break;
                case 2:
                    r *= 1 - s;
                    b *= 1 - s * (1 - f);
                    break;
                case 3:
                    r *= 1 - s;
                    g *= 1 - s * f;
                    break;
                case 4:
                    r *= 1 - s * (1 - f);
                    g *= 1 - s;
                    break;
                case 5:
                    g *= 1 - s;
                    b *= 1 - s * f;
                    break;
            }

            r = (s > 0.0) ? r : v;
            g = (s > 0.0) ? g : v;
            b = (s > 0.0) ? b : v;

            half3 rgb;
            rgb.r = r;
            rgb.g = g;
            rgb.b = b;
            return rgb;
        }

// appdata_tは元のshaderと同じ
                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

// v2fではgrabPosを追加し、GrabPassが使えるようにしている
                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    float4  mask : TEXCOORD2;
                    float4 grabPos : TEXCOORD3; // GrabPass用に追加
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                sampler2D _GrabTexture; // GrabPassを使うと明示したので、UIを描画する直前の画面をこの変数で受け取れる

                fixed4 _Color;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;
                float4 _MainTex_ST;
                float _UIMaskSoftnessX;
                float _UIMaskSoftnessY;
                float _Composition;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    float4 vPosition = UnityObjectToClipPos(v.vertex);
                    OUT.worldPosition = v.vertex;
                    OUT.vertex = vPosition;

                    float2 pixelSize = vPosition.w;
                    pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                    float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                    float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                    OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                    OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                    OUT.grabPos = ComputeGrabScreenPos(OUT.vertex); // GrabPassを使うために追加

                    OUT.color = v.color * _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    //half4 c = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);

                    // GrabPassで取得したテクスチャ
                    half4 c = (tex2D(_GrabTexture, IN.grabPos) + _TextureSampleAdd);
                    // half4 c = half4(1, 1, 1, 1);

                    // 下地となる画面を『a』とする
                    half3 a = c.rgb;

                    // 上から被せるUIを『b』とする
                    half3 b = IN.color.rgb;


                    // 演算式
                    // 通常
                    half3 normal = b;

                    // 乗算
                    half3 mul = a * b;

                    // 加算
                    half3 add = a + b;

                    // 除算
                    half3 div = a / b;

                    // スクリーン
                    half3 scr = 1 - (1 - a) * (1 - b);

                    // オーバーレイ・ハードライト
                    half3 ovly_mul = 2 * a * b;
                    half3 ovly_scr = 1 - 2 * (1 - a) * (1 - b);
                    half3 ovly = step(a, 0.5) * ovly_mul + (1 - step(a, 0.5)) * ovly_scr;
                    half3 hdlgt = step(b, 0.5) * ovly_mul + (1 - step(b, 0.5)) * ovly_scr;

                    // ソフトライト
                    half3 sflgt = (1 - 2 * b) * a * a + 2 * a * b;

                    // 覆い焼きカラー
                    half3 dodge = a / (1 - b);

                    // 焼き込み(リニア)
                    half3 burn = a + b - half3(1, 1, 1);

                    // 差の絶対値
                    half3 diff = abs(a - b);

                    // 比較(暗)
                    half3 cpdark = min(a, b);

                    // 比較(明)
                    half3 cpbrgt = max(a, b);

                    // 色相
                    half3 hue = HSVtoRGB(half3(RGBtoHSV(b).x, RGBtoHSV(a).y, RGBtoHSV(a).z));

                    // 彩度
                    half3 sat = HSVtoRGB(half3(RGBtoHSV(a).x, RGBtoHSV(b).y, RGBtoHSV(a).z));

                    // カラー
                    half3 col = HSVtoRGB(half3(RGBtoHSV(b).x, RGBtoHSV(b).y, RGBtoHSV(a).z));

                    // 輝度
                    half3 lumi = HSVtoRGB(half3(RGBtoHSV(a).x, RGBtoHSV(a).y, RGBtoHSV(b).z));

                    // 合成モードの番号と演算式の対応
                    half4 color0 = half4(normal, IN.color.a); // 0番 = 通常
                    half4 color1 = half4(mul, IN.color.a); // 1番 = 乗算 (以下略)
                    half4 color2 = half4(add, IN.color.a);
                    half4 color3 = half4(div, IN.color.a);
                    half4 color4 = half4(scr, IN.color.a);
                    half4 color5 = half4(ovly, IN.color.a);
                    half4 color6 = half4(hdlgt, IN.color.a);
                    half4 color7 = half4(sflgt, IN.color.a);
                    half4 color8 = half4(dodge, IN.color.a);
                    half4 color9 = half4(burn, IN.color.a);
                    half4 color10 = half4(diff, IN.color.a);
                    half4 color11 = half4(cpdark, IN.color.a);
                    half4 color12 = half4(cpbrgt, IN.color.a);
                    half4 color13 = half4(hue, IN.color.a);
                    half4 color14 = half4(sat, IN.color.a);
                    half4 color15 = half4(col, IN.color.a);
                    half4 color16 = half4(lumi, IN.color.a);

                    float comp = _Composition;

                    // 色の合成
                    half4 color
                        = color0 * step(0, comp) * step(comp, 0)
                        + color1 * step(1, comp) * step(comp, 1)
                        + color2 * step(2, comp) * step(comp, 2)
                        + color3 * step(3, comp) * step(comp, 3)
                        + color4 * step(4, comp) * step(comp, 4)
                        + color5 * step(5, comp) * step(comp, 5)
                        + color6 * step(6, comp) * step(comp, 6)
                        + color7 * step(7, comp) * step(comp, 7)
                        + color8 * step(8, comp) * step(comp, 8)
                        + color9 * step(9, comp) * step(comp, 9)
                        + color10 * step(10, comp) * step(comp, 10)
                        + color11 * step(11, comp) * step(comp, 11)
                        + color12 * step(12, comp) * step(comp, 12)
                        + color13 * step(13, comp) * step(comp, 13)
                        + color14 * step(14, comp) * step(comp, 14)
                        + color15 * step(15, comp) * step(comp, 15)
                        + color16 * step(16, comp) * step(comp, 16)
                        ;


//以下は元のshaderの処理そのまま
                    #ifdef UNITY_UI_CLIP_RECT
                    half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                    color.a *= m.x * m.y;
                    #endif

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(color.a - 0.001);
                    #endif

                    color.rgb *= color.a;

                    return color;
                }
            ENDCG
            }
        }
}

