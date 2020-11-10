using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{

    CameraRender cameraRender = new CameraRender();
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach(Camera camera in cameras)
        {
            cameraRender.Render(context, camera);
        } 
    }
}