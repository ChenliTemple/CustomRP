using UnityEngine;
using UnityEngine.Rendering;

public class CameraRender 
{
    ScriptableRenderContext context;
    Camera camera;
    
    public void Render(ScriptableRenderContext context,Camera camera)
    {
        this.context = context;
        this.camera = camera;

        DrawVisbleGeometry();
        Submit();
    }
    
    public void DrawVisbleGeometry()
    {
        context.DrawSkybox(camera);
    }

    public void Submit()
    {
        context.Submit();
    }
}
