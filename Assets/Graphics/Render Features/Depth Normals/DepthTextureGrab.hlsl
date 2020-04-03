TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

void DepthTextureGrab_float(float2 UV, out Texture2D OutDepthTexture, out float OutDepth)
{
    OutDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, UV).r;
    OutDepthTexture = _CameraDepthTexture;
}