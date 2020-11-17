﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBall : MonoBehaviour
{
    static int baseColorId = Shader.PropertyToID("_BaseColor");
    static int metallicId = Shader.PropertyToID("_Metallic");
    static int smoothnessId = Shader.PropertyToID("_Smoothness");
    
    MaterialPropertyBlock block;

    [SerializeField]
    Mesh mesh = default;

    [SerializeField]
    Material material = default;
    // Start is called before the first frame update

    Matrix4x4[] matrices = new Matrix4x4[1023];
    Vector4[] baseColors = new Vector4[1023];
    float[] metallics = new float[1023];
    float[] smoothness = new float[1023];
    // Update is called once per frame
    private void Awake()
    {
        for(int i = 0; i < matrices.Length; i++)
        {
            matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * 10f, Quaternion.Euler(Random.value * 360f,Random.value * 360f,Random.value * 360f), Vector3.one * Random.Range(0.5f,1.5f));
            baseColors[i] = new Vector4(Random.value, Random.value, Random.value, Random.Range(0.5f,1.0f));
            metallics[i] = Random.value < 0.25f ? 1 : 0;
            smoothness[i] = Random.Range(0.05f, 0.95f);
        }
    }
    void Update()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
            block.SetVectorArray(baseColorId, baseColors);
            block.SetFloatArray(metallicId, metallics);
            block.SetFloatArray(smoothnessId, smoothness);
        }
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, 1023, block);
    }
}
