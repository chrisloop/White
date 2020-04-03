
void AdditionalLights_half(float3 WorldPos, out half3 DirectionOut, out half ShadowAttenOut)
{
    ShadowAttenOut = .5;
    DirectionOut = 1;
    #ifndef SHADERGRAPH_PREVIEW

    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light l = GetAdditionalLight(i, WorldPos); // GET ADDT LIGHTS
        ShadowAttenOut = l.color * l.distanceAttenuation * l.shadowAttenuation;
 
        //Diffuse += Lambert_half(attenColor, l.direction, N);
        //Specular += BlinnPhong_half(attenColor, l.direction, N, V, SpecColor, Smoothness);
    } 
    #endif
}