using UnityEngine;
using UnityEngine.Rendering;


public class WorldSpaceRippleFeature : UnityEngine.Rendering.Universal.ScriptableRendererFeature
{
    class WorldSpaceRipplePass : UnityEngine.Rendering.Universal.ScriptableRenderPass
    {
        private RenderTargetIdentifier source { get; set; }
        private UnityEngine.Rendering.Universal.RenderTargetHandle destination { get; set; }
        public Material rippleMaterial = null;
        UnityEngine.Rendering.Universal.RenderTargetHandle temporaryColorTexture;

        public void Setup(RenderTargetIdentifier source, UnityEngine.Rendering.Universal.RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public WorldSpaceRipplePass(Material rippleMaterial)
        {
            this.rippleMaterial = rippleMaterial;
        }



        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {

        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("WorldSpaceRipple Pass");

            RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDescriptor.depthBufferBits = 0;

            if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
            {
                cmd.GetTemporaryRT(temporaryColorTexture.id, opaqueDescriptor, FilterMode.Point);
                Blit(cmd, source, temporaryColorTexture.Identifier(), rippleMaterial, 0);
                Blit(cmd, temporaryColorTexture.Identifier(), source);

            }
            else Blit(cmd, source, destination.Identifier(), rippleMaterial, 0);

 


            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {

            if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
                cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
        }
    }

    [System.Serializable]
    public class WorldSpaceRippleSettings
    {
        public Material rippleMaterial = null;
    }

    public WorldSpaceRippleSettings settings = new WorldSpaceRippleSettings();
    WorldSpaceRipplePass ripplePass;
    UnityEngine.Rendering.Universal.RenderTargetHandle rippleTexture;

    public override void Create()
    {
        ripplePass = new WorldSpaceRipplePass(settings.rippleMaterial);
        ripplePass.renderPassEvent = UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingPostProcessing;
        rippleTexture.Init("_WorldSpaceRippleTexture");
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
    {
        if (settings.rippleMaterial == null)
        {
            Debug.LogWarningFormat("Missing Outline Material");
            return;
        }
        ripplePass.Setup(renderer.cameraColorTarget, UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget);
        renderer.EnqueuePass(ripplePass);
    }
}


