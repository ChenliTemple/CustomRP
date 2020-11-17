#ifndef CUSTOM_LIT_PASS_INCLUDE
#define CUSTOM_LIT_PASS_INCLUDE

#include "../ShaderLibrary/Common.hlsl"
#include "../ShaderLibrary/Surface.hlsl"
#include "../ShaderLibrary/Light.hlsl"
#include "../ShaderLibrary/BRDF.hlsl"
#include "../ShaderLibrary/Lighting.hlsl"

struct Attributes{
	float3 positionOS : POSITION;
	float3 normalOS : NORMAL;
	float2 baseUV : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings{
	float4 positionCS : SV_POSITION;
	float3 normalWS : VAR_NORMAL;
	float2 baseUV : VAR_BASE_UV;
	float3 positionWS : VAR_POSITION;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
	UNITY_DEFINE_INSTANCED_PROP(float4,_BaseMap_ST)
	UNITY_DEFINE_INSTANCED_PROP(float4,_BaseColor)
	UNITY_DEFINE_INSTANCED_PROP(float,_Cutoff)
	UNITY_DEFINE_INSTANCED_PROP(float,_Metallic)
	UNITY_DEFINE_INSTANCED_PROP(float,_Smoothness)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

Varyings LitPassVertex(Attributes input) 
{
	Varyings output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input,output);
	output.positionWS = TransformObjectToWorld(input.positionOS);
	output.positionCS = TransformWorldToHClip(output.positionWS);
	float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_BaseMap_ST);
	output.baseUV = input.baseUV * baseST.xy + baseST.zw;
	output.normalWS = TransformObjectToWorldNormal(input.normalOS);
	return output;		
}

float4 LitPassFragment(Varyings input) : SV_TARGET 
{
	Surface surface;
	UNITY_SETUP_INSTANCE_ID(input);
	float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,input.baseUV);
	float4 basColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_BaseColor);
	float4 base = basColor * baseMap;
	#if defined(_CLIPPING)
		float cutValue = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_Cutoff);
		clip(base.a - cutValue);
	#endif
	surface.normal = normalize(input.normalWS);
	surface.viewDirection = normalize(_WorldSpaceCameraPos - input.positionWS);
	surface.color = base.rgb;
	surface.alpha = base.a;
	surface.metallic = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_Metallic);
	surface.smoothness = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_Smoothness);

	BRDF brdf;
	#if defined(_PREMULTIPLY_ALPHA)
		brdf = GetBRDF(surface,true);
	#else
		brdf = GetBRDF(surface);
	#endif
	float3 color = GetLighting(surface,brdf);
	return float4(color,surface.alpha);
}



#endif