Shader "Unlit/AdjustablePixelationWithTiling"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _PixelWidth("Pixel Width", Range(1, 2048)) = 320
        _PixelHeight("Pixel Height", Range(1, 2048)) = 180
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
                    float2 uv     : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv  : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST; // Contém scale e offset da textura
                float _PixelWidth;
                float _PixelHeight;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Aplica o scale e offset da textura
                    float2 uv = TRANSFORM_TEX(i.uv, _MainTex);

                    // Agora faz a pixelização com base nas UV transformadas
                    float u = floor(uv.x * _PixelWidth) / _PixelWidth;
                    float v = floor(uv.y * _PixelHeight) / _PixelHeight;

                    float4 col = tex2D(_MainTex, float2(u,v));
                    return col;
                }
                ENDCG
            }
        }

            FallBack "Unlit/Texture"
}
