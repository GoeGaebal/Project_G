// Two-pass box blur shader created for URP 12 and Unity 2021.2
// Made by Alexander Ameye 
// https://alexanderameye.github.io/

Shader "Hidden/Box Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        TEXTURE2D(_MainTex);

        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;

        int _BlurStrength;

        float _Xamplitude = 100;
        float _Yamplitude = 30;
        float _Frequency = 1;
        float _Height = 0.6;

        //Varyings은 쉐이더 사이에 오고 가는 데이터를 나타내는 자료형이다.
        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
            return OUT;
        }
        ENDHLSL

        // pass 0
        Pass
        {
            Name "VERTICAL BOX BLUR"

            HLSLPROGRAM
            half4 frag(Varyings IN) : SV_TARGET
            {
               
               float2 res = _MainTex_TexelSize.xy;

                //float offset = sin(_Time.y*_Frequency)*sin(IN.uv.y*_Yamplitude)/_Xamplitude;
                float offset = sin(_Time.y*10) * sin(IN.uv.y*30) * saturate(0.6-IN.uv.y)/100 ;
                float4 rtn = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(IN.uv.x + offset  , IN.uv.y));

                return rtn;

            }
            ENDHLSL
        }

        // pass 1
        Pass
        {
            Name "HORIZONTAL BOX BLUR"

            HLSLPROGRAM
            half4 frag(Varyings IN) : SV_TARGET         
            {


                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
            }
            ENDHLSL
        }
    }
}