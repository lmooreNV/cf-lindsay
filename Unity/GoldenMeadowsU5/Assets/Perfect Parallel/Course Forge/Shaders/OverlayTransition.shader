Shader "Perfect Parallel/Overlay-Transition" 
{
 Properties
 {
  OMain ("Outer Main Color", Color)  = (1, 1, 1, 1)
  OSpecular ("Outer Specular Color", Color)  = (1, 1, 1, 1)
  [Space]
  [NoScaleOffset]
  ODiffuse ("Outer Texture 1", 2D) = "white" {}
  ODiffuseTile (" ", Float) = 100
  [NoScaleOffset]
  ODetail ("Outer Texture 2", 2D) = "white" {}
  ODetailTile (" ", Float) = 0
  [NoScaleOffset]
  [Normal]
  ONormal ("Outer Normal", 2D) = "bump" {}
  ONormalTile (" ", Float) = 0
  ONormalBump (" ", Range(0, 1)) = 1
  [NoScaleOffset]
  [Normal]
  ODetailNormal ("Outer Detail Normal", 2D) = "bump" {}
  ODetailNormalTile (" ", Float) = 0
  ODetailNormalBump (" ", Range(0, 1)) = 1
  [Space]
  OFresnelIntensity ("Outer Fresnel Intensity", Range(0,1)) = 0.25
  
  IMain ("Inner Main Color", Color)  = (1, 1, 1, 1)
  ISpecular ("Inner Specular Color", Color)  = (1, 1, 1, 1)
  [Space]
  [NoScaleOffset]
  IDiffuse ("Inner Texture 1", 2D) = "white" {}
  IDiffuseTile (" ", Float) = 100
  [NoScaleOffset]
  IDetail ("Inner Texture 2", 2D) = "white" {}
  IDetailTile (" ", Float) = 0
  [NoScaleOffset]
  [Normal]
  INormal ("Inner Normal", 2D) = "bump" {}
  INormalTile (" ", Float) = 0
  INormalBump (" ", Range(0, 1)) = 1
  [NoScaleOffset]
  [Normal]
  IDetailNormal ("Inner Detail Normal", 2D) = "bump" {}
  IDetailNormalTile (" ", Float) = 0
  IDetailNormalBump (" ", Range(0, 1)) = 1
  [Space]
  IFresnelIntensity ("Inner Fresnel Intensity", Range(0,1)) = 0.25
 }
 
 SubShader
 {
  Tags { "Queue" = "AlphaTest+1" }
  Cull Off

  CGPROGRAM	
  #pragma target 3.0	
  #pragma surface surf Standard vertex:vert
  #include "UnityPBSLighting.cginc" 

  #define PP_TRANSITION_INPUT
  #include "Base.cginc" 
  #include "Terrain.cginc" 
  #include "Fresnel.cginc" 
   
  void surf (Input IN, inout SurfaceOutputStandard o) 
  {
   float3 uvw = IN.worldPos;
   float3 terrainColor = ProcessTerrain(uvw);
   
   float3 outerDiffuse = ODiffuseTile == 0 ? terrainColor : ProcessTriplanar(ODiffuse, uvw / ODiffuseTile, IN.customNormal).rgb;
   float3 outerDetail = ODetailTile == 0 ? ONE : ProcessTriplanar(ODetail, uvw / ODetailTile, IN.customNormal).rgb * unity_ColorSpaceDouble.rgb;
   float3 outerNormal = ONormalTile == 0 ? UP : UnpackBump(ProcessTriplanar(ONormal, uvw / ONormalTile, IN.customNormal), ONormalBump);
   float3 outerDetailNormal = ODetailNormalTile == 0 ? UP : UnpackBump(ProcessTriplanar(ODetailNormal, uvw / ODetailNormalTile, IN.customNormal), ODetailNormalBump);
   
   float3 innerDiffuse = IDiffuseTile == 0 ? terrainColor : ProcessTriplanar(IDiffuse, uvw / IDiffuseTile, IN.customNormal).rgb;
   float3 innerDetail = IDetailTile == 0 ? ONE : ProcessTriplanar(IDetail, uvw / IDetailTile, IN.customNormal).rgb * unity_ColorSpaceDouble.rgb;
   float3 innerNormal = INormalTile == 0 ? UP : UnpackBump(ProcessTriplanar(INormal, uvw / INormalTile, IN.customNormal), INormalBump);
   float3 innerDetailNormal = IDetailNormalTile == 0 ? UP : UnpackBump(ProcessTriplanar(IDetailNormal, uvw / IDetailNormalTile, IN.customNormal), IDetailNormalBump);

   o.Albedo = lerp(OMain * outerDiffuse * outerDetail, IMain * innerDiffuse * innerDetail, IN.color.r);
   o.Normal = lerp(BlendNormals(outerNormal, outerDetailNormal), BlendNormals(innerNormal, innerDetailNormal), IN.color.r);
   o.Emission = lerp(OSpecular, ISpecular, IN.color.r) * Fresnel(normalize(_WorldSpaceCameraPos - IN.worldPos), WorldNormalVector(IN, o.Normal), lerp(OFresnelIntensity, IFresnelIntensity, IN.color.r), 10);
   o.Metallic = 0;
   o.Smoothness = 0;

   DetailClip(o.Albedo.r, IN.color.a);
  }
  ENDCG
 }
 
 FallBack "Standard"
}
