
Shader "Custom/HorrorPixelate"
{
    Properties
    {
        _PixelSize ("Pixel Size", Range(1, 12)) = 4
        _GrainStrength ("Film Grain", Range(0, 0.15)) = 0.035
        _DitherStrength ("Dithering", Range(0, 0.15)) = 0.035
        _TintColor ("Horror Tint", Color) = (0.72, 0.82, 1, 1)
        _TintStrength ("Tint Strength", Range(0, 1)) = 0.18
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "PixelateGrainDither"

            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);
            float4 _BlitTexture_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                float _PixelSize;
                float _GrainStrength;
                float _DitherStrength;
                float4 _TintColor;
                float _TintStrength;
            CBUFFER_END

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS =
                    GetFullScreenTriangleVertexPosition(input.vertexID);
                output.uv =
                    GetFullScreenTriangleTexCoord(input.vertexID);
                return output;
            }

            // Простая псевдослучайная функция для зерна.
            float Hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            // Матрица Bayer 4x4 для ретро-дизеринга.
            float Bayer4(float2 p)
            {
                int x = (int)p.x % 4;
                int y = (int)p.y % 4;

                const float bayer[16] = {
                    0,  8,  2, 10,
                    12, 4, 14, 6,
                    3, 11, 1,  9,
                    15, 7, 13, 5
                };

                return (bayer[y * 4 + x] / 16.0) - 0.5;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 screenSize = _BlitTexture_TexelSize.zw;
                float pixelSize = max(_PixelSize, 1.0);
                float2 pixelCount = max(
                    floor(screenSize / pixelSize),
                    float2(1, 1)
                );

                // Пикселизация.
                float2 pixelCoord = floor(input.uv * pixelCount);
                float2 uv = (pixelCoord + 0.5) / pixelCount;

                half4 color = SAMPLE_TEXTURE2D(
                    _BlitTexture,
                    sampler_BlitTexture,
                    uv
                );

                // Холодный цветовой оттенок.
                color.rgb = lerp(
                    color.rgb,
                    color.rgb * _TintColor.rgb,
                    _TintStrength
                );

                // Мелкое анимированное зерно.
                float grain = Hash(
                    pixelCoord + floor(_Time.y * 24.0)
                ) - 0.5;

                color.rgb += grain * _GrainStrength;

                // Лёгкий дизеринг по экранной сетке.
                float2 ditherCoord = floor(input.uv * screenSize);
                float dither = Bayer4(ditherCoord);

                color.rgb += dither * _DitherStrength;

                return half4(saturate(color.rgb), color.a);
            }
            ENDHLSL
        }
    }
}
