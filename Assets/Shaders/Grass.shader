Shader "Custom/Grass" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
	_Speed("MoveSpeed", Range(20,50)) = 25 // speed of the swaying
		_Rigidness("Rigidness", Range(1,50)) = 24// lower makes it look more "liquid" higher makes it look rigid
		_SwayMax("Sway Max", Range(0, 0.1)) = .005 // how far the swaying goes
		_YOffset("Y offset", float) = 1.0// y offset, below this is no animation
		_MaxWidth("Max Displacement Width", Range(0, 2)) = 0.1 // width of the line around the dissolve
		_Radius("Radius", float) = 6 // width of the line around the dissolve
	}
	SubShader {
		Tags { "RenderType"="Opaque" "DisableBatching" = "True" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf CelShadingForward vertex:vert addshadow keepalpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) {
			//half NdotL = dot(s.Normal, lightDir);

			half NdotL = .2;
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

		uniform float3 _Positions[40];
		uniform float _PositionArray;

		struct Input {
			float2 uv_MainTex;
		};

		void vert(inout appdata_full v) {

			//basic swaying 
			float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float x = sin(wpos.x / _Rigidness + (_Time.x * _Speed)) *(v.vertex.z - _YOffset) * 5;// x axis movements
			float z = sin(wpos.y / _Rigidness + (_Time.x * _Speed)) *(v.vertex.z - _YOffset) * 5;// z axis movements
			v.vertex.x += ((step(0, v.vertex.z - _YOffset) * x * _SwayMax))*7;// apply the movement if the vertex's y above the YOffset
			v.vertex.y += ((step(0, v.vertex.z - _YOffset) * z * _SwayMax))*7;

			float clampV = .0049;
			float Radius = 4.7;
			// interaction radius movement for every position in array
			for (int i = 0; i < _PositionArray; i++) {
				float3 dis = distance(_Positions[i], wpos); // distance for radius
				float3 radius = 1 - saturate(dis / Radius); // in world radius based on objects interaction radius
				float3 sphereDisp = wpos - _Positions[i]; // position comparison
				sphereDisp *= radius ; // position multiplied by radius for falloff
				sphereDisp.z *= -1;
				v.vertex.xy += clamp(sphereDisp.xz * step(_YOffset, v.vertex.z) * v.vertex.z * 1.3, -clampV, clampV);// vertex movement based 
				//v.vertex.x += ((step(0, v.vertex.z - _YOffset) * radius.x * sphereDisp.x)) * 7;// apply the movement if the vertex's y above the YOffset
				//v.vertex.y += ((step(0, v.vertex.z - _YOffset) * radius.z * sphereDisp.z)) * 7;
				//v.vertex.x += clamp((step(0, v.vertex.z - _YOffset) * sphereDisp.x * radius.x * 100* (v.vertex.z*6)*(v.vertex.z - _YOffset)), - clampV, clampV);
				//v.vertex.y += clamp((step(0, v.vertex.z - _YOffset) * -sphereDisp.z * radius.z * 100* (v.vertex.z*6)*(v.vertex.z - _YOffset)), -clampV, clampV);
			}
			
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
