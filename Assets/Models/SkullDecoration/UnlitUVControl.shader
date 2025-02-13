Shader "Custom/Unlit_UVControl"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _UVScale("UV Scale (X, Y)", Vector) = (1,1,0,0)
        _UVOffset("UV Offset (X, Y)", Vector) = (0,0,0,0)
        _UseAutoUV("Use Automatic UV? (0 = Manual, 1 = Auto)", Float) = 0
        _Color("Tint Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float3 worldPos : TEXCOORD1;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _UVScale;
                float4 _UVOffset;
                float _UseAutoUV;
                fixed4 _Color;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    if (_UseAutoUV > 0.5)
                    {
                        o.uv = v.worldPos.xz * _UVScale.xy + _UVOffset.xy;
                    }
                    else
                    {
                        o.uv = v.uv * _UVScale.xy + _UVOffset.xy;
                    }

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return tex2D(_MainTex, i.uv) * _Color;
                }
                ENDCG
            }
        }
}
