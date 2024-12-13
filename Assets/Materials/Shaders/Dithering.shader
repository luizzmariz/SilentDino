Shader "Unlit/AdjustableDithering"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _QuantizationLevels("Quantization Levels", Range(2, 256)) = 16
        _DitherIntensity("Dither Intensity", Range(0,1)) = 1.0
        _DitherOffset("Dither Offset", Range(-0.5,0.5)) = 0.0
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
            Pass
            {
                ZWrite Off
                Cull Off
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float _QuantizationLevels;
                float _DitherIntensity;
                float _DitherOffset;

                // Matriz Bayer 4x4 normalizada (0 a 1)
                static const float4x4 bayerMatrix = float4x4(
                    0 / 16.0,  8 / 16.0,  2 / 16.0, 10 / 16.0,
                    12 / 16.0, 4 / 16.0, 14 / 16.0,  6 / 16.0,
                    3 / 16.0, 11 / 16.0,  1 / 16.0,  9 / 16.0,
                    15 / 16.0,7 / 16.0, 13 / 16.0,  5 / 16.0
                );

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float4 col = tex2D(_MainTex, i.uv);
                    float levels = _QuantizationLevels;

                    // Posição na tela para indexar o dither (use floor para garantir índice)
                    float2 pixelPos = floor(i.pos.xy);
                    int bx = (int)fmod(pixelPos.x,4);
                    int by = (int)fmod(pixelPos.y,4);

                    float ditherValue = bayerMatrix[by][bx];

                    // Ajuste do dither:
                    // (ditherValue - 0.5) centraliza o dither em torno de 0
                    // _DitherOffset desloca esse centro
                    // _DitherIntensity controla a amplitude
                    float ditherFactor = (ditherValue - 0.5 + _DitherOffset) * _DitherIntensity;

                    // Quantização com dithering integrado
                    float3 originalColor = col.rgb;
                    float3 ditheredColor = floor((originalColor * levels) + ditherFactor) / (levels - 1.0);

                    return fixed4(ditheredColor, col.a);
                }
                ENDCG
            }
        }
            FallBack "Unlit/Texture"
}
