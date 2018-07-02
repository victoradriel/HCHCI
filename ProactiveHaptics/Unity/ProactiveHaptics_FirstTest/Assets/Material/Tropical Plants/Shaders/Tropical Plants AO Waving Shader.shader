Shader "Custom/TropicalPlantsAOWaving" {
	Properties {
		_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_WaveAndDistance ("Wave, distance and speed", Vector) = (12, 3.6, 1, 1)
		_Cutoff ("Cutoff", float) = 0.5
		_Cutoff2 ("Cutoff2", float) = 0.5
		_AOTex ("Ambient Occlusion (RGB)", 2D) = "white" {}
		_AOFac ("Ambient Occlusion factor", Range (0.5, 2)) = 1
	}
	 
	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="Grass"
		}
		Cull Back
		LOD 200
		ColorMask RGB
		 
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:WavingGrassVert addshadow alphatest:_Cutoff2
		#pragma exclude_renderers flash
		#include "TerrainEngine.cginc"
		 
		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _Shininess;
		fixed _Cutoff;
		sampler2D _AOTex;
		half _AOFac;
		 
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			fixed4 color : COLOR;
			float2 uv2_AOTex;
		};
		 
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			fixed4 d = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 ao = tex2D(_AOTex, IN.uv2_AOTex);
			ao.rgb = ((ao.rgb - 0.5f) * max(_AOFac, 0)) + 0.5f;
			o.Albedo = c.rgb * ao.rgb;
			o.Alpha = d.a;
			clip (o.Alpha - _Cutoff);
			o.Gloss = d.a;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
 
	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="Grass"
		}
		Cull Back
		LOD 200
		ColorMask RGB
		 
		Pass {
			Tags { "LightMode" = "Vertex" }
			Material {
				Diffuse (1,1,1,1)
				Ambient (1,1,1,1)
                Specular [_SpecColor]
			}
			Lighting On
			ColorMaterial AmbientAndDiffuse
			AlphaTest Greater [_Cutoff]
			SetTexture [_MainTex] { combine texture * primary DOUBLE, texture }
		}
		Pass {
			Tags { "LightMode" = "VertexLMRGBM" }
			AlphaTest Greater [_Cutoff]
			BindChannels {
				Bind "Vertex", vertex
				Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
				Bind "texcoord", texcoord1 // main uses 1st uv
			}
			SetTexture [unity_Lightmap] {
				matrix [unity_LightmapMatrix]
				combine texture * texture alpha DOUBLE
			}
			SetTexture [_MainTex] { combine texture * previous QUAD, texture }
		}
	}
	 
	Fallback Off
}