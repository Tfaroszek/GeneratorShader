Shader "Custom/ShadowShader"
{
    Properties
    {
        _ShadowMap ("Shadow Map", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
            };

            sampler2D _ShadowMap;
            float4x4 _LightVP;
            float _ShadowBias;
            float3 _LightDir;

            // Funkcja wierzchołków, przekształca dane wierzchołków do przestrzeni klipu
            v2f vert (appdata_t v)
            {
                v2f o;
                // Przekształcenie wierzchołka do przestrzeni klipu
                o.pos = UnityObjectToClipPos(v.vertex);
                // Obliczenie pozycji w świecie
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // Normalizacja wektora normalnego
                o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

                // Przekształcenie do przestrzeni światła
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                // Obliczenie współrzędnych cienia w przestrzeni światła
                o.shadowCoord = mul(_LightVP, worldPos);
                // Projekcja perspektywiczna
                o.shadowCoord.xyz /= o.shadowCoord.w; 

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Pobranie dynamicznego kierunku światła
                float3 lightDir = normalize(_LightDir);

                // Oświetlenie (dyfuzja)
                float diff = max(dot(i.normal, lightDir), 0.0);

                // Transformacja do UV
                float2 shadowUV = i.shadowCoord.xy * 0.5 + 0.5;
                shadowUV = saturate(shadowUV); // Ograniczenie do 0-1

                // Pobranie głębokości z mapy cienia
                float shadowDepth = tex2D(_ShadowMap, shadowUV).r;

                // Bias, aby uniknąć self-shadowing
                float bias = 0.005;

                // Sprawdzenie, czy piksel jest w cieniu
                float shadow = (shadowDepth + bias < i.shadowCoord.z) ? 1.0 : 0.01;

                return fixed4(diff * shadow, diff * shadow, diff * shadow, 1);
            }
            ENDCG
        }
    }
}
