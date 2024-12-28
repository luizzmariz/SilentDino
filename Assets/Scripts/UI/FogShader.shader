Shader "Custom/FogShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogIntensity("Fog Intensity", Range(0, 1)) = 0.5
        _FogSpeed("Fog Speed", Range(0, 5)) = 1
    }
        SubShader
        {
            Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            Pass
            {
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
                float4 _MainTex_ST;
                float4 _FogColor;
                float _FogIntensity;
                float _FogSpeed;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Base texture
                    fixed4 col = tex2D(_MainTex, i.uv);

                // Fog effect
                float noise = sin(i.uv.x * 10 + _Time.y * _FogSpeed) *
                              cos(i.uv.y * 10 - _Time.y * _FogSpeed);
                float fogAmount = saturate(noise * _FogIntensity);
                fixed4 fogColor = lerp(col, _FogColor, fogAmount);

                return fogColor;
            }
            ENDCG
        }
        }
            FallBack "Unlit/Transparent"
}
