Shader "Custom/NewSurfaceShader"
{
      Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Xamplitude ("x 진폭, 숫자랑 반비례", float) = 100
        _Yamplitude ("y 진폭, 숫자랑 반비례", float) = 30
        _Frequency ("주기, 숫자랑 비례", float) =4
        _Height("아지랑이 적용되는 높이", Range(0,1)) = 0.6


    }
    SubShader
    {
        Cull Off		//! 컬링하지 않는다. 
		ZWrite Off		//! Z Buffer 사용하지 않는다.
		ZTest Always	//! ZTest 항상 위에 출력

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
				float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            float _Xamplitude;
            float _Yamplitude;
            float _Frequency;
            float _Height;
            fixed4 frag (v2f i) : SV_Target
            {
				//float4 fMainTex = tex2D(_MainTex, i.uv);		//! 이전 버퍼 텍스처
				//float3 fGrayTex = dot(fMainTex.rgb, float3(0.3333f, 0.3333f, 0.3333f));	//! 흑백 계산


                float offset = sin(_Time.y*_Frequency)*sin(i.uv.y*_Yamplitude)/_Xamplitude;

                return tex2D(_MainTex,float2(i.uv.x + offset * saturate(_Height - i.uv.y) , i.uv.y));

            }
            ENDCG
        }
    }
}
