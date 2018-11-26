Shader "Custom/MapBlender" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture 1", 2D) = "white" {}
		_SecondaryTex("Texture 2", 2D) = "white"{}
		_BlendMap("Blend Map (Grayscale)", 2D) = "black"{}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf CelShadingForward

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) {
		half NdotL = step(0,dot(s.Normal, lightDir));
		float darkness = .17;
		float lightness = .2 - darkness;
		NdotL = darkness + (NdotL*lightness);

		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);

		c.a = s.Alpha;
		return c;
	}

	sampler2D _MainTex;
	sampler2D _SecondaryTex;
	sampler2D _BlendMap;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BlendMap;
	};

	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutput o) {
		// Albedo comes from a texture tinted by color
		fixed4 tex1blend = fixed4(1, 1, 1, 1) - tex2D(_BlendMap, IN.uv_BlendMap);
		tex1blend.w = 1;
		fixed4 c = ((tex2D(_MainTex, IN.uv_MainTex) * tex1blend) + (tex2D(_SecondaryTex, IN.uv_MainTex) * tex2D(_BlendMap, IN.uv_BlendMap))) * _Color;



		o.Albedo = c.rgb;

		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
