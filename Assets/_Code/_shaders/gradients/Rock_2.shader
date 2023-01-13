// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Rock_2"
{
	Properties
	{
		_Basecolor("Base color ", Color) = (0,0,0,0)
		_AmbientGlow("Ambient Glow", Color) = (1,0.5514706,0.950507,0)
		_Colorrim("Color rim", Color) = (0.8161765,0.9923934,1,0)
		_Bias("Bias", Range( 0 , 1)) = 0.5
		_Power("_Power", Range( 0 , 10)) = 1
		_Intensity("Intensity", Range( 0 , 20)) = 0.5
		_GlossIntensity("Gloss Intensity", Range( 0 , 2)) = 0
		_Spec("Spec", Range( 0 , 2)) = 0
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _Basecolor;
		uniform float4 _AmbientGlow;
		uniform float4 _Colorrim;
		uniform float _Bias;
		uniform float _Intensity;
		uniform float _Power;
		uniform float _Spec;
		uniform float _GlossIntensity;

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = tex2D( _TextureSample0, i.uv_texcoord ).rgb;
			o.Albedo = _Basecolor.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV22 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode22 = ( _Bias + _Intensity * pow( 1.0 - fresnelNdotV22, (12.0 + (_Power - 0.0) * (0.0 - 12.0) / (12.0 - 0.0)) ) );
			float4 lerpResult28 = lerp( _AmbientGlow , _Colorrim , fresnelNode22);
			float4 temp_cast_2 = (0.0).xxxx;
			float4 temp_cast_3 = (0.9).xxxx;
			float4 clampResult29 = clamp( lerpResult28 , temp_cast_2 , temp_cast_3 );
			o.Emission = clampResult29.rgb;
			o.Specular = _Spec;
			o.Gloss = _GlossIntensity;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf BlinnPhong keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
785;58;663;409;3226.85;1402.289;6.0006;True;True
Node;AmplifyShaderEditor.RangedFloatNode;18;-1462.554,314.223;Float;False;Property;_Power;_Power;7;0;Create;True;0;0;False;0;1;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1461.554,233.2227;Float;False;Property;_Intensity;Intensity;8;0;Create;True;0;0;False;0;0.5;7.4;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;19;-1162.599,359.6136;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;12;False;3;FLOAT;12;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1462.554,152.2228;Float;False;Property;_Bias;Bias;6;0;Create;True;0;0;False;0;0.5;0.99;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;26;-942.0616,-227.2222;Float;False;Property;_AmbientGlow;Ambient Glow;1;0;Create;True;0;0;False;0;1,0.5514706,0.950507,0;0.6320754,0.1773621,0.128204,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;22;-970.552,111.2228;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;27;-941.4996,-58.97491;Float;False;Property;_Colorrim;Color rim;2;0;Create;True;0;0;False;0;0.8161765,0.9923934,1,0;0.6792453,0.4715163,0.2723389,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;28;-597.9778,1.871788;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-672.4359,211.4291;Float;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-680.6133,280.935;Float;False;Constant;_Float1;Float 1;12;0;Create;True;0;0;False;0;0.9;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;32;-981.6201,-527.0516;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;33;-656.7599,-486.2216;Inherit;True;Property;_TextureSample0;Texture Sample 0;12;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-1858.912,680.1552;Float;False;Property;_EdgeGlow;Edge Glow;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;29;-498.671,190.9862;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-2178.912,968.1551;Float;False;Property;_Power1;Power 1;5;0;Create;True;0;0;False;0;2;2;-3;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1586.912,728.1552;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-496.5132,519.1282;Float;False;Property;_Spec;Spec;10;0;Create;True;0;0;False;0;0;0.81;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-495.4895,606.6583;Float;False;Property;_GlossIntensity;Gloss Intensity;9;0;Create;True;0;0;False;0;0;1.4;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;8;-2242.912,776.1552;Inherit;False;PerturbNormal;-1;;18;c8b64dd82fb09f542943a895dffb6c06;1,26,0;1;6;FLOAT3;0,0,0;False;4;FLOAT3;9;FLOAT;28;FLOAT;29;FLOAT;30
Node;AmplifyShaderEditor.ColorNode;3;-437.8285,-257.9572;Float;False;Property;_Basecolor;Base color ;0;0;Create;True;0;0;False;0;0,0,0,0;0.6509434,0.05219828,0.05219828,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;2;-1390.197,650.4631;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;17;-1858.912,504.1552;Float;False;Property;_EdgeAmbient;Edge Ambient;3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;6;-1842.912,872.1553;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;BlinnPhong;Rock_2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;11;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;18;0
WireConnection;22;1;21;0
WireConnection;22;2;20;0
WireConnection;22;3;19;0
WireConnection;28;0;26;0
WireConnection;28;1;27;0
WireConnection;28;2;22;0
WireConnection;33;1;32;0
WireConnection;29;0;28;0
WireConnection;29;1;30;0
WireConnection;29;2;31;0
WireConnection;4;0;5;0
WireConnection;4;1;6;0
WireConnection;2;0;17;0
WireConnection;2;1;4;0
WireConnection;6;2;7;0
WireConnection;0;0;3;0
WireConnection;0;1;33;0
WireConnection;0;2;29;0
WireConnection;0;3;1;0
WireConnection;0;4;16;0
ASEEND*/
//CHKSM=B7F8E1C90DCFE28FE29CB83B761799F4A7024168