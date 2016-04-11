#if defined(PP_CORE_INPUT)
float CFresnelIntensity;
float CFresnelFalloff;
#elif defined (PP_TRANSITION_INPUT)
float OFresnelIntensity;
float OFresnelFalloff;
float IFresnelIntensity;
float IFresnelFalloff;
#endif

float Fresnel(float3 viewDir, float3 normal, float FresnelIntensity, float FresnelFalloff)
{
   float fresnel = 1 - dot (viewDir, normal);
   return pow(fresnel, FresnelFalloff) * FresnelIntensity;
}