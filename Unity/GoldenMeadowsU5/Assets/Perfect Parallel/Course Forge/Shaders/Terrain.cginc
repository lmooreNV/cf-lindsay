uniform float4 TerrainInfo;
uniform sampler2D TerrainTexture;

float3 ProcessTerrain(float3 worldPos)
{
 return tex2D(TerrainTexture, worldPos.xz / TerrainInfo.xz).rgb;
}