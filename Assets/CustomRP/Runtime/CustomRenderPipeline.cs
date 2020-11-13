using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    bool useDynamicBatching, useGPUInstancing;
    public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing,bool useSPRBatcher)
    {
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSPRBatcher;
    }
    
    CameraRender cameraRender = new CameraRender();
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach(Camera camera in cameras)
        {
            cameraRender.Render(context, camera,useDynamicBatching,useGPUInstancing);
        } 
    }
}