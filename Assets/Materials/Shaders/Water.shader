Shader "Custom/PSXWaterSewer"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (0.2, 0.35, 0.2, 1.0)
        _WaveSpeed("Wave Speed", Range(0, 2)) = 0.2
        _WaveScale("Wave Scale", Range(0, 0.1)) = 0.05
        _ColorSteps("Color Steps", Range(2, 32)) = 8
        _DitherAmount("Dither Amount", Range(0, 1)) = 0.2
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
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
                    UNITY_FOG_COORDS(1) // Coordenadas da neblina
                };

                sampler2D _MainTex;
                float4 _BaseColor;
                float _WaveSpeed;
                float _WaveScale;
                float _ColorSteps;
                float _DitherAmount;

                // Quantiza cores (estilo PSX)
                float3 QuantizeColor(float3 col, float steps)
                {
                    return round(col * steps) / steps;
                }

                // Movimento das ondas
                v2f vert(appdata v)
                {
                    v2f o;
                    float wave = sin(_Time.y * _WaveSpeed + v.vertex.x * 10.0) * _WaveScale;
                    v.vertex.y += wave; // Adiciona ondulação
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;

                    UNITY_TRANSFER_FOG(o, o.pos); // Transfere as coordenadas de neblina
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Textura com movimento sutil
                    float2 uv = i.uv;
                    uv.y += sin(_Time.y * _WaveSpeed) * 0.02;

                    // Amostra a textura base e aplica cor
                    fixed4 baseTex = tex2D(_MainTex, uv) * _BaseColor;

                    // Quantiza a cor para obter efeito "PSX"
                    float3 quantizedColor = QuantizeColor(baseTex.rgb, _ColorSteps);

                    // Aplica dithering
                    int2 pixelCoord = int2(i.pos.xy);
                    float dither = frac(sin(dot(pixelCoord, float2(12.9898, 78.233))) * 43758.5453);
                    quantizedColor += (dither - 0.5) * _DitherAmount * (1.0 / _ColorSteps);
                    quantizedColor = saturate(quantizedColor);

                    // Aplica a neblina
                    fixed4 finalColor = fixed4(quantizedColor, baseTex.a);
                    UNITY_APPLY_FOG(i.fogCoord, finalColor);

                    return finalColor;
                }
                ENDCG
            }
        }
            FallBack "Transparent"
}
