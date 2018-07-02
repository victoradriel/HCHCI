Shader "Custom/TransparentGlow" 
{
	Properties 
	{
		_ColorTint("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (1, 0, 0, 1)
		_RimStrength("Rim Strength", Range(1.0, 6.0)) = 3.0
	}
	SubShader 
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
				
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		struct Input 
		{
			float4 color : Color;
			float2 uv_MainTex;
			float3 viewDir;
		};

		fixed4 _ColorTint;
		sampler2D _MainTex;
		float4 _RimColor;
		float _RimStrength;

		void surf (Input IN, inout SurfaceOutput o) 
		{
			//IN.color = _ColorTint;
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _ColorTint;
			//o.Albedo = c.rgb * IN.color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
						
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimStrength);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
