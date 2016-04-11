Shader "Perfect Parallel/Overlay-Core" 
{
 Properties
 {
  CMain ("Main Color", Color)  = (1, 1, 1, 1)
  CSpecular ("Specular Color", Color)  = (1, 1, 1, 1)
  [Space]
  [NoScaleOffset]
  CDiffuse ("Texture 1", 2D) = "white" {}
  CDiffuseTile (" ", Float) = 100
  [NoScaleOffset]
  CDetail ("Texture 2", 2D) = "white" {}
  CDetailTile (" ", Float) = 0
  [NoScaleOffset]
  [Normal]
  CNormal ("Normal", 2D) = "bump" {}
  CNormalTile (" ", Float) = 0
  CNormalBump (" ", Range(0, 1)) = 1
  [NoScaleOffset]
  [Normal]
  CDetailNormal ("Detail Normal", 2D) = "bump" {}
  CDetailNormalTile (" ", Float) = 0
  CDetailNormalBump (" ", Range(0, 1)) = 1
  [Space]
  CFresnelIntensity ("Fresnel Intensity", Range(0,1)) = 0.25
 }
 
 SubShader
 {
  Tags { "Queue" = "AlphaTest+2" }
  Cull Off

  CGPROGRAM
  #pragma target 3.0       
  #pragma surface surf Standard vertex:vert
  #include "UnityPBSLighting.cginc" 
  
  #define PP_CORE_INPUT
  #include "Base.cginc" 
  #include "Terrain.cginc" 
  #include "Fresnel.cginc" 

  void surf (Input IN, inout SurfaceOutputStandard o) 
  {
   float3 uvw = IN.worldPos;
   float3 terrainColor = ProcessTerrain(uvw);
   
   float3 coreDiffuse = CDiffuseTile == 0 ? terrainColor : ProcessTriplanar(CDiffuse, uvw / CDiffuseTile, IN.customNormal).rgb;
   float3 coreDetail = CDetailTile == 0 ? ONE : ProcessTriplanar(CDetail, uvw / CDetailTile, IN.customNormal).rgb * unity_ColorSpaceDouble.rgb;
   float3 coreNormal = CNormalTile == 0 ? UP : UnpackBump(ProcessTriplanar(CNormal, uvw / CNormalTile, IN.customNormal), CNormalBump);
   float3 coreDetailNormal = CDetailNormalTile == 0 ? UP : UnpackBump(ProcessTriplanar(CDetailNormal, uvw / CDetailNormalTile, IN.customNormal), CDetailNormalBump);

   o.Albedo = CMain * coreDiffuse * coreDetail;
   o.Normal = BlendNormals(coreNormal, coreDetailNormal);

   o.Emission = CSpecular * Fresnel(normalize(_WorldSpaceCameraPos - IN.worldPos), WorldNormalVector (IN, o.Normal), CFresnelIntensity, 10);
   o.Metallic = 0;
   o.Smoothness = 0;

   DetailClip(o.Albedo.r, IN.color.a);
  }
  ENDCG
 }
 
 FallBack "Standard"
}
