Shader "Custom/UnlitTransparentScroll" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ScrollXSpeed ("X Scroll Speed", Range (0,10)) = 2
	}
	SubShader {
		Lighting Off
       	AlphaTest Greater 0.5
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Unlit

		fixed4 LightingUnlit(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}
		sampler2D _MainTex;
		fixed _ScrollXSpeed;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			fixed2 scrolledUV = IN.uv_MainTex;
			fixed xscrollValue = _ScrollXSpeed * _Time;

			scrolledUV += fixed2( xscrollValue, 0);

			half4 c = tex2D (_MainTex, scrolledUV);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
