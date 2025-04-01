Shader "Custom/DepthShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
            };

            v2f vert(float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.depth = o.pos.z / o.pos.w; // Normalizacja głębokości
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(i.depth, i.depth, i.depth, 1);
            }
            ENDCG
        }
    }
}
