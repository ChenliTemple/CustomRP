#ifndef CUSTOM_BRDF_INCLUDE
#define CUSTOM_BRDF_INCLUDE

#define MIN_REFLECTIVITY 0.04

struct BRDF {
	float3 diffuse;
	float3 specular;
	float roughness;
};

float OneMinusRefectivity(float metallic) {
	float range = 1.0 - MIN_REFLECTIVITY;
	return range - metallic * range;
}

BRDF GetBRDF(Surface surface) {
	BRDF brdf;
	float oneMinusRefectivity = OneMinusRefectivity(surface.metallic);
	brdf.diffuse = surface.color * oneMinusRefectivity;
	brdf.specular = 0.0;
	brdf.roughness = 1.0;
	return brdf;
}



#endif