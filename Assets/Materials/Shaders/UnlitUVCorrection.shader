Shader "Custom/UnlitWorldUVFog" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}            // Textura principal
        _UseLocalUV("Use Local UV", Float) = 1.0        // 1 = usar UV do modelo; 0 = usar projeção mundial
        _Rotate90("Rotate UV 90", Float) = 0.0          // 1 = rotacionar UVs em 90 graus
        _AlbedoTint("Albedo Tint", Color) = (1,1,1,1)   // Multiplicador de cor (albedo)
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            Pass {
                Cull Off // Habilita renderização dos dois lados

                CGPROGRAM
                #pragma target 4.5
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _UseLocalUV;
                float _Rotate90;
                float4 _AlbedoTint;

                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv  : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                    float3 worldNormal : TEXCOORD2;
                };

                v2f vert(appdata_full v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.texcoord.xy;
                    float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.worldPos = worldPos;
                    float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.worldNormal = normalize(worldNormal);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float2 uv;
                    if (_UseLocalUV > 0.5) {
                        uv = i.uv;
                    }
     else {
      float3 n = normalize(i.worldNormal);
      float3 absN = abs(n);
      if (absN.x >= absN.y && absN.x >= absN.z) {
          uv = i.worldPos.yz;
      }
else if (absN.y >= absN.x && absN.y >= absN.z) {
 uv = float2(i.worldPos.x, i.worldPos.z);
}
else {
 uv = i.worldPos.xy;
}
}

if (_Rotate90 > 0.5) {
    uv = float2(uv.y, 1.0 - uv.x);
}

uv = uv * _MainTex_ST.xy + _MainTex_ST.zw;

fixed4 texColor = tex2D(_MainTex, uv);
texColor *= _AlbedoTint;

float dist = length(_WorldSpaceCameraPos - i.worldPos);

float fogFactor;
#if defined(FOG_LINEAR)
    fogFactor = dist * unity_FogParams.z + unity_FogParams.w;
#elif defined(FOG_EXP)
    fogFactor = exp2(-unity_FogParams.y * dist);
#elif defined(FOG_EXP2)
    float expTerm = unity_FogParams.x * dist;
    fogFactor = exp2(-expTerm * expTerm);
#else
    fogFactor = 1.0;
#endif

fogFactor = saturate(fogFactor);

fixed4 finalColor = lerp(unity_FogColor, texColor, fogFactor);
return finalColor;
}
ENDCG
}
        }
}
