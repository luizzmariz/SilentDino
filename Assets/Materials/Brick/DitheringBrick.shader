Shader "Unlit/DitheringBayer"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _QuantizationLevels("Quantization Levels", Range(2, 256)) = 16
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

                // Matriz Bayer 4x4 normalizada (0 a 1)
                static const float4x4 bayerMatrix = float4x4(
                    0 / 16.0,  8 / 16.0,  2 / 16.0, 10 / 16.0,
                    12 / 16.0, 4 / 16.0, 14 / 16.0,  6 / 16.0,
                    3 / 16.0, 11 / 16.0,  1 / 16.0,  9 / 16.0,
                    15 / 16.0,7 / 16.0,  13 / 16.0,  5 / 16.0
                );

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                float3 QuantizeColor(float3 color, float levels)
                {
                    // Quantiza o valor (por exemplo, reduzindo as cores)
                    return floor(color * levels) / (levels - 1.0);
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;
                    float4 col = tex2D(_MainTex, uv);

                    // Índice na matriz Bayer
                    int x = (int)(floor(fmod(i.pos.x,4)));
                    int y = (int)(floor(fmod(i.pos.y,4)));

                    float threshold = bayerMatrix[y][x];

                    // Primeiro quantizamos a cor
                    float3 quantColor = QuantizeColor(col.rgb, _QuantizationLevels);

                    // Agora decidimos se aplicaremos dithering
                    // A ideia: se o pixel original é mais claro que o quantizado + um offset, acende um pouco mais.
                    // Caso contrário, mantém quantColor. Assim criamos o efeito pontilhado.
                    float3 ditheredColor = quantColor;

                    // Para simular a perda de cor, consideramos o valor original e o quantizado.
                    // Se o original for maior que o quantizado, e passar do threshold, ajustamos um degrau a mais.
                    float3 originalColor = col.rgb;
                    float3 diff = originalColor - quantColor;

                    // Se a diferença for maior que o threshold do dithering, incrementa um nível
                    if (diff.r > threshold / _QuantizationLevels) ditheredColor.r += (1.0 / (_QuantizationLevels - 1.0));
                    if (diff.g > threshold / _QuantizationLevels) ditheredColor.g += (1.0 / (_QuantizationLevels - 1.0));
                    if (diff.b > threshold / _QuantizationLevels) ditheredColor.b += (1.0 / (_QuantizationLevels - 1.0));

                    return fixed4(ditheredColor, col.a);
                }
                ENDCG
            }
        }
            FallBack "Unlit/Texture"
}
