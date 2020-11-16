﻿using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    const int maxDirLigthCount = 4;
    static int dirLightCountId = Shader.PropertyToID("_DirectionalLightCount");
    static int dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors");
    static int dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");
    static Vector4[] dirLightColors = new Vector4[maxDirLigthCount];
    static Vector4[] dirLightDirections = new Vector4[maxDirLigthCount];
    

    const string bufferName = "Lighting";
    ScriptableRenderContext context;
    CullingResults cullingResults;
    CommandBuffer buffer = new CommandBuffer()
    {
        name = bufferName
    };

    public void Setup(ScriptableRenderContext context,CullingResults cullingResults)
    {
        this.context = context;
        this.cullingResults = cullingResults;

        buffer.BeginSample(bufferName);
        SetupLights();
        buffer.EndSample(bufferName);
        Submit();
    }

    public void Submit()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void SetupLights()
    {
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;

        int dirLightCount = 0;
        for(int i = 0;i < visibleLights.Length; i++)
        {
            VisibleLight visibleLight = visibleLights[i];
            if (visibleLight.lightType == LightType.Directional)
            {
                SetupDirectionalLight(dirLightCount++, ref visibleLight);
                if (dirLightCount >= maxDirLigthCount)
                {
                    break;
                }
            }
        }
        buffer.SetGlobalInt(dirLightCountId, visibleLights.Length);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
    }

    void SetupDirectionalLight(int index,ref VisibleLight visibleLight)
    {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }
}
