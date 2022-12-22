// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyStencil/FresnelTexturedAlpha"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		_Basecolor1("Base color 1", Color) = (0,0,0,0)
		_Glowcolor1("Glow color 1", Color) = (0,0,0,0)
		_Power1("Power 1", Range( -3 , 6)) = 2
		_StencilReferenceID("Stencil Reference ID", Int) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Float) = 3
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Float) = 0
		_StencilReadMask("Stencil Read Mask", Range( 0 , 255)) = 255
		_StencilWriteMask("Stencil Write Mask", Range( 0 , 255)) = 255
		_MainTexture("MainTexture", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		Stencil
		{
			Ref [_StencilReferenceID]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComparison]
			PassFront [_StencilOperation]
		}
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
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

		uniform float _StencilComparison;
		uniform float _StencilReadMask;
		uniform float _StencilWriteMask;
		uniform float _StencilOperation;
		uniform int _StencilReferenceID;
		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform float4 _Basecolor1;
		uniform float4 _Glowcolor1;
		uniform sampler2D _Normal;
		uniform float _Power1;
		uniform float _Cutoff = 0.2;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv0_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float4 tex2DNode29 = tex2D( _MainTexture, uv0_MainTexture );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV6 = dot( BlendNormals( ase_worldNormal , UnpackNormal( tex2D( _Normal, uv0_MainTexture ) ) ), ase_worldViewDir );
			float fresnelNode6 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV6, _Power1 ) );
			o.Emission = ( ( tex2DNode29 * _Basecolor1 ) + ( _Glowcolor1 * fresnelNode6 ) ).rgb;
			o.Alpha = 1;
			clip( tex2DNode29.a - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

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
Version=18000
1967;127;1700;793;1204.398;673.8094;1.3;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-1528.1,-331.9097;Inherit;False;0;29;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;33;-1343.796,259.491;Inherit;True;Property;_Normal;Normal;10;0;Create;True;0;0;False;0;-1;None;7419d7b946f7a644f9d2b8a6a8ef79bb;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;37;-1235.894,96.9905;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BlendNormalsNode;38;-1030.193,188.0905;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1023.176,287.1555;Float;False;Property;_Power1;Power 1;3;0;Create;True;0;0;False;0;2;0.98;-3;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;6;-667.5734,194.4335;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;29;-624.9051,-374.3519;Inherit;True;Property;_MainTexture;MainTexture;9;0;Create;True;0;0;False;0;-1;None;8c6e562f9b1c1fc47aeffa2fd078a50a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-663.7729,7.633362;Float;False;Property;_Glowcolor1;Glow color 1;2;0;Create;True;0;0;False;0;0,0,0,0;0.2358491,0.2358491,0.2358491,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-465.4995,-155.792;Float;False;Property;_Basecolor1;Base color 1;1;0;Create;True;0;0;False;0;0,0,0,0;0.5377358,0.5377358,0.5377358,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-209.4399,-348.665;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-352.0262,29.23916;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-333.133,-740.3955;Float;False;Property;_StencilOperation;Stencil Operation;6;1;[Enum];Create;True;0;1;UnityEngine.Rendering.StencilOp;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;16;-274.2784,-917.5092;Float;False;Property;_StencilReferenceID;Stencil Reference ID;4;0;Create;True;0;0;True;0;1;1;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-333.0155,-827.2779;Float;False;Property;_StencilComparison;Stencil Comparison;5;1;[Enum];Create;True;0;1;UnityEngine.Rendering.CompareFunction;True;0;3;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-310.133,-643.3954;Float;False;Property;_StencilReadMask;Stencil Read Mask;7;0;Create;True;0;0;True;0;255;0;0;255;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-306.133,-556.3954;Float;False;Property;_StencilWriteMask;Stencil Write Mask;8;0;Create;True;0;0;True;0;255;255;0;255;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;2;34.7312,-350.1389;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;213.9096,-396.5013;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;AmplifyStencil/FresnelTexturedAlpha;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.2;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;True;0;True;16;255;True;21;255;True;22;0;True;17;0;True;18;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;33;1;39;0
WireConnection;38;0;37;0
WireConnection;38;1;33;0
WireConnection;6;0;38;0
WireConnection;6;3;7;0
WireConnection;29;1;39;0
WireConnection;32;0;29;0
WireConnection;32;1;3;0
WireConnection;4;0;5;0
WireConnection;4;1;6;0
WireConnection;2;0;32;0
WireConnection;2;1;4;0
WireConnection;0;2;2;0
WireConnection;0;10;29;4
ASEEND*/
//CHKSM=46A79EDE080F1AACAC2C936B2E944791EEFEA7AB