Shader "Custom/Bumped Diffuse Ambient Occlusion" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_AOTex ("Ambient Occlusion (RGB)", 2D) = "white" {}
		_AOFac ("Ambient Occlusion factor", Range (0.5, 2)) = 1
		_Emis ("Emission factor", Range (0, 1)) = 0
	}
	SubShader { 
		Tags { "RenderType"="Opaque" }
		LOD 400

	CGPROGRAM
	#pragma surface surf BlinnPhong
	#pragma target 3.0

	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _AOTex;
	fixed4 _Color;
	half _AOFac;
	half _Emis;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float2 uv2_AOTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 ao = tex2D(_AOTex, IN.uv2_AOTex);
		ao.rgb = ((ao.rgb - 0.5f) * max(_AOFac, 0)) + 0.5f;
		o.Albedo = tex.rgb * _Color.rgb * ao.rgb;
		o.Alpha = tex.a * _Color.a;
		o.Emission = tex.rgb * _Emis;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
	ENDCG
	}

	FallBack "Diffuse"
}