using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRender 
{
    ScriptableRenderContext context;
    Camera camera;
    CullingResults cullResults;
    Lighting lighting = new Lighting();

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");
    
    const string bufferName = "Render Camera";
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };
    
    public void Render(ScriptableRenderContext context,Camera camera,bool useDynamicBatching,bool useGPUInstancing,ShadowSettings shadowSettings)
    {
        this.context = context;
        this.camera = camera;

        PrepareBuffer();
        PrepareForSceneWindow();
        if (!Cull(shadowSettings.maxDistance))
            return;

        SetUp();
        buffer.BeginSample(SampleName);
        ExecuteBuffer();
        lighting.Setup(context,cullResults,shadowSettings);
        buffer.EndSample(SampleName);
        DrawVisbleGeometry(useDynamicBatching,useGPUInstancing);
        DrawUnsupportedShaders();
        DrawGizmos();
        lighting.Cleanup();
        Submit();
    }

    public bool Cull(float maxShadowDistance)
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)){
            p.shadowDistance = Mathf.Min(camera.farClipPlane, maxShadowDistance);
            cullResults = context.Cull(ref p);
            return true;
        }
        return false;
    }
    
    public void DrawVisbleGeometry(bool useDynamicBatcing, bool useGPUInstancing)
    {
        SortingSettings sortingSettings = new SortingSettings(camera) {
            criteria = SortingCriteria.CommonOpaque
        };
        DrawingSettings drawSetting = new DrawingSettings(unlitShaderTagId, sortingSettings) {
            enableDynamicBatching = useDynamicBatcing,
            enableInstancing = useGPUInstancing
        };
        drawSetting.SetShaderPassName(1, litShaderTagId);
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        context.DrawRenderers(cullResults, ref drawSetting, ref filteringSettings);
        context.DrawSkybox(camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawSetting.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cullResults, ref drawSetting, ref filteringSettings);
    }

    public void SetUp()
    {
        context.SetupCameraProperties(camera);
        CameraClearFlags cFlags = camera.clearFlags;
        buffer.ClearRenderTarget(cFlags <= CameraClearFlags.Depth, cFlags == CameraClearFlags.Color, cFlags == CameraClearFlags.SolidColor ? camera.backgroundColor.linear : Color.clear);
        buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }

    public void Submit()
    {
        buffer.EndSample(SampleName);
        ExecuteBuffer();
        context.Submit();
    }

    public void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
