Shader "Unlit/Fog"
{
	Properties
	{
		_Tint("Fog Tint", Color) = (1, 1, 1, .5)
		_Strength("Fog Strength", Range(0,.1)) = 0.05
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Overlay" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 scrPos : TEXCOORD2;
				float2 uv_MainTex : TEXCOORD0;
            };

			float4 _Tint;
			uniform sampler2D _CameraDepthTexture;
			float _Strength;
			sampler2D _MainTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.uv_MainTex = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{				
				//get depth
				half depth =  LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
				//set density based on screen position and strength
				half4 fog = (_Strength * (depth - i.scrPos.w));
				//apply color
				half4 col = tex2D(_MainTex, i.uv_MainTex) * _Tint;
				col.w = fog * col.w;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
