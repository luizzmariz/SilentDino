Shader "Custom/PixelationShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _PixelSize("Pixel Size", Float) = 10.0
        _CensorArea("Censor Area", Vector) = (0.5, 0.5, 0.2, 0.2) // (x, y, width, height)
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

                struct appdata
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
                float _PixelSize;
                float4 _CensorArea;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;

                    // Verifica se o UV está dentro da área de censura
                    if (uv.x > _CensorArea.x && uv.x < _CensorArea.x + _CensorArea.z &&
                        uv.y > _CensorArea.y && uv.y < _CensorArea.y + _CensorArea.w)
                    {
                        // Calcula o efeito de pixelização
                        float pixelSize = _PixelSize / _ScreenParams.x;
                        uv = floor(uv / pixelSize) * pixelSize;
                    }

                    fixed4 col = tex2D(_MainTex, uv);
                    return col;
                }
                ENDCG
            }
        }
}