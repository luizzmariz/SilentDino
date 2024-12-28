Shader "Custom/HauntedTextureShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Color("Tint Color", Color) = (0.5, 0, 0.5, 1)
        _Speed("Scroll Speed", Float) = 1.0
        _Distortion("Distortion Strength", Float) = 0.2
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

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
                float4 _Color;
                float _Speed;
                float _Distortion;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Moving UVs with distortion
                    float2 scroll = i.uv + _Speed * _Time.y;
                    float2 noise = (tex2D(_MainTex, scroll).rg - 0.5) * _Distortion;
                    float2 distortedUV = i.uv + noise;

                    // Main texture color
                    fixed4 texColor = tex2D(_MainTex, distortedUV);
                    return texColor * _Color;
                }
                ENDCG
            }
        }
}
