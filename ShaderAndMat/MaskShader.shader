Shader "Gondr/MaskShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        // 이걸로 색 버퍼에 아무것도 안 쓰게 함
        ColorMask 0  
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // 어차피 ColorMask 0이므로 이 색은 출력되지 않음
                return half4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }
}
