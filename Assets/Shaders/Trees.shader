Shader "Custom/Trees" {
			Properties{
			_Color("Color", Color) = (1,1,1,1)
			_MainTex("Albedo (RGB)", 2D) = "white" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
		_Speed("MoveSpeed", Range(20,50)) = 25 // speed of the swaying
			_Rigidness("Rigidness", Range(1,50)) = 44// lower makes it look more "liquid" higher makes it look rigid
			_SwayMax("Sway Max", Range(0, 0.1)) = .005 // how far the swaying goes
			_YOffset("Y offset", float) = 1.0// y offset, below this is no animation
			_MaxWidth("Max Displacement Width", Range(0, 2)) = 0.1 // width of the line around the dissolve
			_Radius("Radius", float) = 6 // width of the line around the dissolve
		}
			SubShader{
			Tags{ "RenderType" = "Opaque" "DisableBatching" = "True" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf CelShadingForward vertex:vert addshadow keepalpha

			// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = step(0,dot(s.Normal, lightDir)-.2);
			float darkness = .16;
			float lightness = .2 - darkness;
			NdotL = darkness + (NdotL*lightness);
			//half NdotL = .2;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;
		float _Radius;

		float _Speed;
		float _SwayMax;
		float _YOffset;
		float _Rigidness;
		float _MaxWidth;



		struct Input {
			float2 uv_MainTex;
		};

		void vert(inout appdata_full v) {

			//basic swaying 
			float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float x = sin(wpos.x / _Rigidness + (_Time.x * _Speed)) *(v.vertex.y - _YOffset) * 5;// x axis movements
			float z = sin(wpos.y / _Rigidness + (_Time.x * _Speed)) *(v.vertex.y - _YOffset) * 5;// z 
			
			
			float y = sin(1*(wpos.z*wpos.z + wpos.x*wpos.x) / _Rigidness + (_Time.x * _Speed)) *(v.vertex.x - _YOffset) * 5;
			v.vertex.x += ((step(0, v.vertex.y - _YOffset) * x * _SwayMax))*1;// apply the movement if the vertex's y above the YOffset
			v.vertex.z += ((step(0, v.vertex.y - _YOffset) * -z * _SwayMax)) * 1;
			v.vertex.y += ((step(0, v.vertex.y - _YOffset) * abs(v.vertex.x) * y * _SwayMax)) * 10;

			float clampV = .0049;
			float Radius = 4.7;
		}



		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
		}
			FallBack "Diffuse"
	}
