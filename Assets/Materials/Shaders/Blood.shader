Shader "Custom/BloodFluidEffect"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TimeScale("Speed", Float) = 1.0
        _DistortionStrength("Distortion Strength", Float) = 0.2
        _FluidNoise("Fluid Noise", 2D) = "white" {}
        _CustomTime("Custom Time", Float) = 0.0 // Valor de tempo customizado
    }

        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
            LOD 100

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Off
                ZWrite Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                sampler2D _FluidNoise;
                float4 _MainTex_ST;
                float _TimeScale;
                float _DistortionStrength;
                float _CustomTime; // Tempo customizado

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;

                    // Use o tempo customizado para distorção
                    float time = _CustomTime * _TimeScale;
                    float2 noiseUV = uv + time * float2(0.1, -0.5);

                    // Sample noise texture for fluid dynamics
                    float2 noise = tex2D(_FluidNoise, noiseUV).rg * 2.0 - 1.0;
                    noise *= _DistortionStrength;

                    // Apply distortion
                    uv += noise;

                    // Wrap UV to keep it seamless
                    uv = frac(uv);

                    // Sample the main texture
                    fixed4 col = tex2D(_MainTex, uv);

                    // Add transparency for blending effect
                    col.a *= step(uv.y, 1.0); // Gradual fade-out at the bottom

                    return col;
                }
                ENDCG
            }
        }
}
