// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "TerrainShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_HeightTex ("Height (R)", 2D) = "white" {}
		_TerrainSize ("Terrain Size", Vector) = (300, 50, 300, 0)
		_TextureSize ("Texture Size", Vector) = (640, 480, 0, 0)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma only_renderers d3d9
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _HeightTex;

		float4 _TerrainSize;
		float4 _TextureSize;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void vert (inout appdata_full v) 
		{
			// Calculate the world position of the vertex.
			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

			// Calculate a texture step.
			float textureStepX = 1.0f / _TextureSize.x;
            float textureStepY = 1.0f / _TextureSize.y;

			//  Calculate the texture positions at x and the surrounding points.
			//  0 - 1   7
            //  | \ | \ |
            //  5 - x - 2
            //  | \ | \ |
            //  6 - 4 - 3

			float4 texturePos = float4(worldPos.x / _TerrainSize.x, worldPos.z / _TerrainSize.z, 0, 1);
			//texturePos.x = 1 - texturePos.x;
			//texturePos.y = 1 - texturePos.y;

			// Adjust x and y because the height texture is not completely filled.
			// x: 640 / 1024 = 0.625
			// y: 480 / 512 = 0.9375

			texturePos.x *= 0.625;
			texturePos.y *= 0.9375;


			// Calculate texture coordinates.
			float4 north = texturePos + float4(0 , -textureStepY, 0, 0);
            float4 south = texturePos + float4(0 , textureStepY, 0, 0);
            float4 east = texturePos + float4(textureStepX, 0, 0, 0);
            float4 west = texturePos + float4(-textureStepX, 0, 0, 0);

			// Sample the textures.
			fixed2 sampleBase = tex2Dlod(_HeightTex, texturePos).rg;
            fixed2 northSample = tex2Dlod(_HeightTex, north).rg;
            fixed2 southSample = tex2Dlod(_HeightTex, south).rg;
            fixed2 eastSample = tex2Dlod(_HeightTex, east).rg;
            fixed2 westSample = tex2Dlod(_HeightTex, west).rg;

			// Sample height values.		
			// Example: float heightBase = (sampleBase.g * 256 * 255 + sampleBase.r * 255) / 65536;

			float heightBase = (sampleBase.g * 65280 + sampleBase.r * 255) / 65536;
            float northHeight = (northSample.g * 65280 + northSample.r * 255) / 65536;
            float southHeight = (southSample.g * 65280 + southSample.r * 255) / 65536;
            float eastHeight = (eastSample.g * 65280 + eastSample.r * 255) / 65536;
            float westHeight = (westSample.g * 65280 + westSample.r * 255) / 65536;

			// Difference in x and z direction.
			float3 southNorth = float3(0, (northHeight - southHeight) * _TerrainSize.y, -2);
			//float3 southNorth = float3(0, 0, -2);
			float3 eastWest = float3(-2, (westHeight - eastHeight) * _TerrainSize.y, 0);

			// Calculate the normal.
			v.normal = normalize(cross(southNorth, eastWest));

			// Adjust the y-component of the vertex.
			v.vertex.y = heightBase * _TerrainSize.y;

			// Output the texture coordinates for texturing of the terrain.
			v.texcoord = float4(0.5, heightBase, 0, 0);
			//v.texcoord = float4(v.normal.xz, 0, 0);
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			//half4 c = tex2D (_HeightTex, IN.uv_MainTex);
			o.Albedo = c.rgb; // color
			//o.Albedo = IN.uv_MainTex.y; // height
			//o.Albedo = half3(IN.uv_MainTex.x, 0, IN.uv_MainTex.y); // normal
			//o.Albedo = float3(IN.uv_MainTex, 0);
			//o.Albedo = 1;

			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
