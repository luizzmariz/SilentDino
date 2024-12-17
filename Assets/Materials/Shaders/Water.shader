Shader "Custom/PSXWaterSewer"
{
    Properties
    {
        _MainTex("Base Texture (optional)", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (0.2, 0.35, 0.2, 1.0) // Verde escuro
        _DitherAmount("Dither Amount", Range(0,1)) = 0.3
        _ColorSteps("Color Steps", Range(2,32)) = 8
        _WaveSpeed("Wave Speed", Range(0,2)) = 0.2
        _WaveScale("Wave Scale", Range(0,1)) = 0.05
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Lighting Off
            Cull Off

            Pass
            {
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
                    float2 uv  : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _BaseColor;
                float _DitherAmount;
                float _WaveSpeed;
                float _WaveScale;
                float _ColorSteps;

                // Matriz de dithering 4x4 (típica)
                static const float2 ditherMat[16] = {
                    float2(0,0), float2(8,0), float2(2,0), float2(10,0),
                    float2(12,0), float2(4,0), float2(14,0), float2(6,0),
                    float2(3,0), float2(11,0), float2(1,0), float2(9,0),
                    float2(15,0), float2(7,0), float2(13,0), float2(5,0)
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    // Adicionar uma leve oscilação na posição da água para simular movimento
                    float timeFactor = _Time.y * _WaveSpeed;
                    float wave = sin(v.vertex.x * 10.0 + timeFactor) * _WaveScale;
                    v.vertex.y += wave;

                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                // Função para quantizar uma cor com base em um número de steps
                float3 QuantizeColor(float3 col, float steps)
                {
                    return round(col * steps) / steps;
                }

                float GetDitherValue(int x, int y)
                {
                    // Calcula índice do pixel no padrão 4x4
                    int xi = (x % 4);
                    int yi = (y % 4);
                    int idx = yi * 4 + xi;
                    return ditherMat[idx].x / 16.0;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Amostra a textura base
                    float2 uv = i.uv;
                    float timeFactor = _Time.y * _WaveSpeed * 0.5;
                    // Movimento sutil da textura (como se água estivesse fluindo)
                    uv.y += timeFactor * 0.1;
                    uv.x += timeFactor * 0.05;

                    float4 baseTex = tex2D(_MainTex, uv) * _BaseColor;

                    // Quantização da cor (cores reduzidas)
                    float3 quantCol = QuantizeColor(baseTex.rgb, _ColorSteps);

                    // Aplicar dithering
                    // Calcula o valor de dithering baseado na posição da tela
                    int2 pixelCoord = int2(i.pos.xy);
                    float ditherVal = GetDitherValue(pixelCoord.x, pixelCoord.y);

                    // Ajusta a cor de acordo com o dithering
                    // A ideia é empurrar a cor um pouco para cima ou para baixo dependendo do padrão
                    float3 finalCol = quantCol + (ditherVal - 0.5) * _DitherAmount * (1.0 / _ColorSteps);

                    // Clampa para evitar sair do intervalo [0,1]
                    finalCol = saturate(finalCol);

                    return float4(finalCol, baseTex.a);
                }
                ENDCG
            }
        }
            FallBack "Transparent"
}
