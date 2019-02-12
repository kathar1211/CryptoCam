//https://lindenreid.wordpress.com/2017/12/15/simple-water-shader-in-unity/

Shader "Custom/Water"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
	_TileFactor("Tiling", int) = 1
		//_RampTex("Ramp Texture", 2D) = "white" {}
		_EdgeColor("Edge Color", Color) = (1,1,1,1)
		_DepthFactor("Depth Factor", float) = 1.0
		_NoiseTex("Texture", 2D) = "white" {}
	_WaterOpacity("Water Opacity", Range(0,1)) = 0.5
		_WaveSpeed("Wave Speed", float) = 1.0
		_WaveAmp("Wave Amp", float) = 0.2
		_AnimSpeed("Texture Animation", float) = 1.0
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" }
		LOD 200
		Cull Off
		GrabPass
	{
		"_BackgroundTexture"
	}

		Pass
	{

		CGPROGRAM
		// required to use ComputeScreenPos()
#include "UnityCG.cginc"

#pragma vertex vert
#pragma fragment frag

		// Unity built-in - NOT required in Properties
		sampler2D _CameraDepthTexture;
	sampler2D _MainTex;
	sampler2D _NoiseTex;

	sampler2D _BackgroundTexture;

	fixed4 _Color;
	fixed4 _EdgeColor;
	float _DepthFactor;
	float _TileFactor;
	float _WaterOpacity;
	float _WaveSpeed;
	float _WaveAmp;
	float _AnimSpeed;

	//new properties from https://www.patreon.com/posts/24192529
	uniform float3 _Position;
	uniform sampler2D _GlobalEffectRT;
	uniform float _OrthographicCamSize;
	uniform float _RippleYPos;


	struct vertexInput
	{
		float4 vertex : POSITION;
		float4 texCoord : TEXCOORD1;
	};

	struct vertexOutput
	{
		float4 pos : SV_POSITION;
		float4 texCoord : TEXCOORD0;
		float4 screenPos : TEXCOORD1;
		float4 grabPos : TEXCOORD2;
		float4 worldPos : TEXCOORD4;
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		// convert obj-space position to camera clip space
		output.pos = UnityObjectToClipPos(input.vertex);

		// apply wave animation
		float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
		output.pos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp;
		output.pos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp;

		// compute depth (screenPos is a float4)
		output.screenPos = ComputeScreenPos(output.pos);

		output.grabPos = ComputeGrabScreenPos(output.pos);

		output.texCoord = input.texCoord;

		output.worldPos = mul(unity_ObjectToWorld, input.vertex);

		return output;
	}

	float4 frag(vertexOutput input) : COLOR
	{
		// sample camera depth texture
		float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
		float depth = LinearEyeDepth(depthSample).r;

		// Because the camera depth texture returns a value between 0-1,
		// we can use that value to create a grayscale color
		// to test the value output.
		float4 grayDepth = float4(depth / 255, depth / 255, depth / 255, 1);
		float foamline = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));
		foamline = step(_DepthFactor, foamline);

		//float4 foamRamp = float4(tex2D(_RampTex, float2(foamLine, 0.5)).rgb, 1.0);
		float4 color = _Color;
		//float4 color = _Color * foamRamp;

		//adding ripples //https://www.patreon.com/posts/24192529
		float2 uv = input.worldPos.xz - _Position.xz;
		uv = uv / (_OrthographicCamSize * 2);
		uv += .5;



		float ripples = tex2D(_GlobalEffectRT, uv).b;
		ripples = step(.99, ripples * 2.5);
		//color += (ripples*_EdgeColor);

		//use y position to determine whether or not there should be any ripples
		ripples *= step(input.worldPos.y - 5, _RippleYPos);


		float4 albedo = tex2D(_MainTex, input.texCoord.xy * _TileFactor + _Time * _AnimSpeed);
		//return tex2Dproj(_BackgroundTexture, input.grabPos) * (grayDepth) * albedo * color;
		//return tex2Dproj(_BackgroundTexture, input.grabPos) * (albedo * color * 10) + (foamline * _EdgeColor);
		return (tex2Dproj(_BackgroundTexture, input.grabPos)*(1 - _WaterOpacity)) + (albedo * color * _WaterOpacity) + (foamline * _EdgeColor) + (ripples*_EdgeColor);

		//return  (grayDepth + color) * albedo;
	}

		ENDCG
	}
	}
}