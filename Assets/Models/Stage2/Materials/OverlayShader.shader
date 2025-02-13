Shader "Custom/BloodOverlayShader_Darkness_Fog"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _BloodTex("Blood Texture", 2D) = "white" {}
        _DissolveTex("Dissolve Texture", 2D) = "gray" {}
        _Blend("Blend Amount", Range(0,1)) = 0
        _Darkness("Darkness Factor", Range(0.6,1)) = 0.6
        _EnableFog("Enable Fog? (0 = Off, 1 = On)", Float) = 0
        _MainTexScale("Base Texture Scale (X, Y)", Vector) = (1,1,0,0)
        _BloodScale("Blood Texture Scale (X, Y)", Vector) = (1,1,0,0)
        _DissolveScale("Dissolve Texture Scale", Float) = 1.0
        _DissolveThreshold("Dissolve Start", Range(0,1)) = 0.8
        _DissolveSoftness("Dissolve Softness", Range(0.01,0.5)) = 0.1
        _UseLocalUV("Use Local UV Mapping?", Float) = 0
        _MainColor("Base Color", Color) = (1,1,1,1)
        _BloodColor("Blood Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float3 worldPos : TEXCOORD0;
                    float3 worldNormal : TEXCOORD1;
                    float2 localUV : TEXCOORD2;
                    float4 vertex : SV_POSITION;
                    UNITY_FOG_COORDS(3)
                };

                sampler2D _MainTex;
                sampler2D _BloodTex;
                sampler2D _DissolveTex;
                float _Blend;
                float _Darkness;
                float _EnableFog;
                float _DissolveThreshold;
                float _DissolveSoftness;
                float _UseLocalUV;
                float4 _MainTexScale;
                float4 _BloodScale;
                float _DissolveScale;
                fixed4 _MainColor;
                fixed4 _BloodColor;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                    o.localUV = v.uv;
                    UNITY_TRANSFER_FOG(o, o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 mainUV, bloodUV, dissolveUV;

                    if (_UseLocalUV > 0.5)
                    {
                        mainUV = i.localUV * _MainTexScale.xy;
                        bloodUV = i.localUV * _BloodScale.xy;
                        dissolveUV = i.localUV * _DissolveScale;
                    }
                    else
                    {
                        float3 absNormal = abs(i.worldNormal);
                        if (absNormal.y > absNormal.x && absNormal.y > absNormal.z)
                        {
                            mainUV = i.worldPos.xz * _MainTexScale.xy;
                            bloodUV = i.worldPos.xz * _BloodScale.xy;
                            dissolveUV = i.worldPos.xz * _DissolveScale;
                        }
                        else if (absNormal.x > absNormal.y && absNormal.x > absNormal.z)
                        {
                            mainUV = i.worldPos.yz * _MainTexScale.xy;
                            bloodUV = i.worldPos.yz * _BloodScale.xy;
                            dissolveUV = i.worldPos.yz * _DissolveScale;
                        }
                        else
                        {
                            mainUV = i.worldPos.xy * _MainTexScale.xy;
                            bloodUV = i.worldPos.xy * _BloodScale.xy;
                            dissolveUV = i.worldPos.xy * _DissolveScale;
                        }
                    }

                    fixed4 baseColor = tex2D(_MainTex, mainUV) * _MainColor;
                    fixed4 bloodColor = tex2D(_BloodTex, bloodUV) * _BloodColor;

                    float dissolve = tex2D(_DissolveTex, dissolveUV).r;

                    float dissolveEffect = smoothstep(_DissolveThreshold - _DissolveSoftness, _DissolveThreshold + _DissolveSoftness, dissolve);
                    float blendFactor = smoothstep(0, 1, _Blend) * dissolveEffect;

                    fixed4 finalColor = lerp(baseColor, bloodColor, blendFactor);
                    finalColor.rgb *= (1.0 - _Darkness);

                    if (_EnableFog > 0.5)
                    {
                        UNITY_APPLY_FOG(i.fogCoord, finalColor);
                    }

                    return finalColor;
                }
                ENDCG
            }
        }
}
