using Assets.Utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class BackgroundRenderEffect : MonoBehaviour
{
    Camera cam;
    [SerializeField ] Shader shader;
    [SerializeField ] Material material;
    ScriptableRenderer render;
   [SerializeField]  CameraRenderPass renderPass;
    void Start()
    {
        cam = GetComponent<Camera>();
        renderPass = new CameraRenderPass(material);
     
        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        
    }

    private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext arg1, Camera cam_)
    {
        if (cam == cam_)
        {
            
            //
            /* Debug.Log($"Beginning rendering the camera: {cam_.name}");*/
            render = cam.GetUniversalAdditionalCameraData().scriptableRenderer;
            renderPass.SetRenderColor(render);
            render.EnqueuePass(renderPass);
            
        }

    }

    // Update is called once per frame
    void Update()
    {
            
    }
    
}

class CameraRenderPass : ScriptableRenderPass
{
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("ColorBlit");
    Material mat;
    public RTHandle CameraColor;
    public CameraRenderPass(Material m) {
        mat = m;
        
    }
    ScriptableRenderer renderer;
   public void SetRenderColor (ScriptableRenderer r)
    {
        renderer = r;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {

        CommandBuffer cmd = CommandBufferPool.Get();
   
  
            Blitter.BlitCameraTexture(cmd, renderer.cameraColorTargetHandle, renderer.cameraColorTargetHandle, mat,0);
       
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        cmd.Release();
     
    }
}