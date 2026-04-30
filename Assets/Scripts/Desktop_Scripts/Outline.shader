Shader "Hidden/Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineWidth ("Outline Width", Float) = 0.03
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Front
        ZWrite On
        ZTest Less

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _OutlineWidth;
            float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            // v2f vert(appdata v)
            // {
            //     v2f o;
            //     float3 norm = normalize(v.normal);
            //     v.vertex.xyz += norm * _OutlineWidth;
            //     o.pos = UnityObjectToClipPos(v.vertex);
            //     return o;
            // }
            v2f vert(appdata v) {
                v2f o;

                float3 norm = normalize(v.normal);

                // convert to view space
                float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, norm);

                // push outward in screen-consistent direction
                float2 offset = normalize(viewNormal.xy) * _OutlineWidth;

                float4 pos = UnityObjectToClipPos(v.vertex);
                pos.xy += offset;

                o.pos = pos;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}