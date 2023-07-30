// Made with Amplify Shader Editor v1.9.1.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RCCP Car Body Shader"
{
	Properties
	{
		_DiffuseTexture("Diffuse Texture", 2D) = "white" {}
		_DiffuseColor("Diffuse Color", Color) = (0,0,0,0)
		_MetallicIntensity("Metallic Intensity", Range( 0 , 1)) = 0
		_SmoothnessIntensity("Smoothness Intensity", Range( 0 , 1)) = 0
		_FlakesTexture("Flakes Texture", 2D) = "white" {}
		_FlakesColor("Flakes Color", Color) = (0,0,0,0)
		_FlakesIntensity("Flakes Intensity", Range( 0 , 2)) = 0
		_FlakesRimPower("Flakes Rim Power", Range( 0 , 1)) = 0.46
		_FlakesPower("Flakes Power", Range( 0 , 10)) = 0.16
		_FresnelColor("Fresnel Color", Color) = (1,0,0,0)
		_FresnelPower("Fresnel Power", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _FresnelColor;
		uniform float4 _DiffuseColor;
		uniform sampler2D _DiffuseTexture;
		uniform float4 _DiffuseTexture_ST;
		uniform float _FresnelPower;
		uniform float _FlakesIntensity;
		uniform sampler2D _FlakesTexture;
		uniform float4 _FlakesTexture_ST;
		uniform float4 _FlakesColor;
		uniform float _FlakesRimPower;
		uniform float _FlakesPower;
		uniform float _MetallicIntensity;
		uniform float _SmoothnessIntensity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_DiffuseTexture = i.uv_texcoord * _DiffuseTexture_ST.xy + _DiffuseTexture_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV223 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode223 = ( 0.1 + 1.0 * pow( 1.0 - fresnelNdotV223, _FresnelPower ) );
			float4 lerpResult221 = lerp( _FresnelColor , ( _DiffuseColor * tex2D( _DiffuseTexture, uv_DiffuseTexture ) ) , fresnelNode223);
			o.Albedo = lerpResult221.rgb;
			float2 uv_FlakesTexture = i.uv_texcoord * _FlakesTexture_ST.xy + _FlakesTexture_ST.zw;
			float fresnelNdotV32 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode32 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV32, ( 1.0 - _FlakesRimPower ) ) );
			float4 temp_cast_1 = (_FlakesPower).xxxx;
			o.Emission = pow( ( ( _FlakesIntensity * ( tex2D( _FlakesTexture, uv_FlakesTexture ) * _FlakesColor ) ) * ( 1.0 - fresnelNode32 ) ) , temp_cast_1 ).rgb;
			o.Metallic = _MetallicIntensity;
			o.Smoothness = _SmoothnessIntensity;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
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
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
Version=19103
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;74.66112,-1447.005;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RCCP Car Body Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SamplerNode;38;-1015.085,-2293.452;Inherit;True;Property;_DiffuseTexture;Diffuse Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;58;-933.0099,-2477.855;Inherit;False;Property;_DiffuseColor;Diffuse Color;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0.3920174,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;222;-657.7601,-2363.817;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;221;-416.7394,-2535.243;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;223;-694.1541,-2212.351;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;225;-994.8953,-2077.42;Inherit;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;220;-927.4731,-2677.871;Inherit;False;Property;_FresnelColor;Fresnel Color;9;0;Create;True;0;0;0;False;0;False;1,0,0,0;0,1,0.7987695,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;183;-305.6917,-1171.199;Inherit;False;Property;_MetallicIntensity;Metallic Intensity;2;0;Create;True;0;0;0;False;0;False;0;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-298.6715,-1090.248;Inherit;False;Property;_SmoothnessIntensity;Smoothness Intensity;3;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;32;-1392.843,-1074.387;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;192;-1579.324,-913.1327;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1878.282,-872.7606;Inherit;True;Property;_FlakesRimPower;Flakes Rim Power;7;0;Create;True;0;0;0;False;0;False;0.46;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;213;-750.1962,-1203.111;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;214;-976.9836,-1076.488;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;158;-1588.191,-1281.525;Inherit;False;Property;_FlakesColor;Flakes Color;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;204;-1308.666,-1354.902;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1323.743,-1474.056;Inherit;False;Property;_FlakesIntensity;Flakes Intensity;6;0;Create;True;0;0;0;False;0;False;0;1.5;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;-974.4586,-1378.722;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;86;-329.4342,-1397.461;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-631.6932,-1105.988;Inherit;True;Property;_FlakesPower;Flakes Power;8;0;Create;True;0;0;0;False;0;False;0.16;6;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;189;-1672.256,-1495.79;Inherit;True;Property;_FlakesTexture;Flakes Texture;4;0;Create;True;0;0;0;False;0;False;-1;None;acc2168de4ee88749a498cfd3c6a5f07;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;224;-921.317,-1984.56;Inherit;False;Property;_FresnelPower;Fresnel Power;10;0;Create;True;0;0;0;False;0;False;1;0.2;0;1;0;1;FLOAT;0
WireConnection;0;0;221;0
WireConnection;0;2;86;0
WireConnection;0;3;183;0
WireConnection;0;4;60;0
WireConnection;222;0;58;0
WireConnection;222;1;38;0
WireConnection;221;0;220;0
WireConnection;221;1;222;0
WireConnection;221;2;223;0
WireConnection;223;1;225;0
WireConnection;223;3;224;0
WireConnection;32;3;192;0
WireConnection;192;0;33;0
WireConnection;213;0;209;0
WireConnection;213;1;214;0
WireConnection;214;0;32;0
WireConnection;204;0;189;0
WireConnection;204;1;158;0
WireConnection;209;0;36;0
WireConnection;209;1;204;0
WireConnection;86;0;213;0
WireConnection;86;1;87;0
ASEEND*/
//CHKSM=7253847298B508EB1C3246415F78F6A0DBDED09C