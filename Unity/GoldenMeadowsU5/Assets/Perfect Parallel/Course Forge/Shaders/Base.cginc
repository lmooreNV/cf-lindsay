#define ZERO float3(0, 0, 0)
#define ONE float3(1, 1, 1)
#define UP float3(0, 0, 1)

struct Input
{
 float4 color:COLOR;
 float3 customNormal;

 float3 worldPos;
 float3 worldNormal;
 INTERNAL_DATA
};

void vert (inout appdata_full v, out Input o)
{
 UNITY_INITIALIZE_OUTPUT(Input, o);
 o.customNormal = normalize(v.normal);
}

#if defined(PP_CORE_INPUT)
float4 CMain;
float4 CSpecular;
sampler2D CDiffuse;
sampler2D CDetail;
sampler2D CNormal;
sampler2D CDetailNormal;
sampler2D CNoise;
float CDiffuseTile;
float CDetailTile;
float CNormalTile;
float CDetailNormalTile;
float CNoiseTile;
float CNormalBump;
float CDetailNormalBump;
#elif defined (PP_TRANSITION_INPUT)
float4 OMain;
float4 OSpecular;
sampler2D ODiffuse;
sampler2D ODetail;
sampler2D ONormal;
sampler2D ODetailNormal;
sampler2D ONoise;
float ODiffuseTile;
float ODetailTile;
float ONormalTile;
float ODetailNormalTile;
float ONoiseTile;
float ONormalBump;
float ODetailNormalBump;

float4 IMain;
float4 ISpecular;
sampler2D IDiffuse;
sampler2D IDetail;
sampler2D INormal;
sampler2D IDetailNormal;
sampler2D INoise;
float IDiffuseTile;
float IDetailTile;
float INormalTile;
float IDetailNormalTile;
float INoiseTile;
float INormalBump;
float IDetailNormalBump;
#endif

float4 ProcessTriplanar(in sampler2D texSampler, float3 uvw, float3 normal)
{
 half3 weights = pow (abs(normal), 1);
 weights = weights / max(weights.x + weights.y + weights.z, 0.00001);

 return tex2D(texSampler, float2(uvw.z * sign(normal.x), uvw.y)) * weights.xxxx + tex2D(texSampler, -float2(uvw.x, uvw.z)) * weights.yyyy + tex2D(texSampler, float2(uvw.x * sign(-normal.z), uvw.y)) * weights.zzzz;
}

float3 UnpackBump(float4 normal, float bump)
{
	if (all(normal) == 0 || bump == 0) return float3(0, 0, 1);
	return UnpackScaleNormal(normal, bump);
}

void DetailClip(float color, float alpha)
{
 if(alpha < 0.95) clip(color * alpha - 0.1);
}