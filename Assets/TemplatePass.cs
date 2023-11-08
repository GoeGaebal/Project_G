
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


//ScriptableRenderPass를 상속받는다!!!
public class TemplatePass : ScriptableRenderPass
{
    // The profiler tag that will show up in the frame debugger.
    //디버그 할 때 구분하기 위한 이름
    const string ProfilerTag = "Template Pass";

    // We will store our pass settings in this variable.
    TemplateFeature.PassSettings passSettings;
    
    //Render Target 은 color, depth 등 rendering pipeline의 과정에서 나오는 결과물을 저장하는데 사용된다.
    RenderTargetIdentifier colorBuffer, temporaryBuffer;
    int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");
    
    Material material;
    
    // It is good to cache the shader property IDs here.
    static readonly int BlurStrengthPropertyID = Shader.PropertyToID("_BlurStrength");
    
    // 생성자
    // The constructor of the pass. Here you can set any material properties that do not need to be updated on a per-frame basis.
    public TemplatePass(TemplateFeature.PassSettings passSettings)
    {
        this.passSettings = passSettings;

        // Set the render pass event.
        renderPassEvent = passSettings.renderPassEvent; 
        
        // We create a material that will be used during our pass. You can do it like this using the 'CreateEngineMaterial' method, giving it
        // a shader path as an input or you can use a 'public Material material;' field in your pass settings and access it here through 'passSettings.material'.
        //내가 지정한 shader로 material을 만든다.
        if(material == null) material = CoreUtils.CreateEngineMaterial( "Hidden/Box Blur");
        
        // Set any material properties based on our pass settings.
        //feature에서 정한 property의 값으로 material의 값을 정한다. 
        material.SetInt(BlurStrengthPropertyID, passSettings.blurStrength);
    }

    // rendering pass가 실행되기 전에 매번 실행된다.
    // Gets called by the renderer before executing the pass.
    // Can be used to configure render targets and their clearing state.
    // Can be used to create temporary render target textures.
    // If this method is not overriden, the render pass will render to the active camera render target.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        // RenderTextureDescriptor 구조체에는 width, height 등 render textrure에 대한 정보가 저장되어 있다.
        // renderingData.cameraData.cameraTargetDescriptor에는 방금 카메라가 렌더링한 텍스쳐의 정보가 저장되어 있다.
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        
        // Downsample the original camera target descriptor. 
        // You would do this for performance reasons or less commonly, for aesthetics.
        //성능을 위해 카메라에 렌더링 된 텍스쳐를 다운샘플링
        descriptor.width /= passSettings.downsample;
        descriptor.height /= passSettings.downsample;
        
        // Set the number of depth bits we need for our temporary render texture.
        //depth buffer 사용 안 한다는 뜻
        descriptor.depthBufferBits = 0;
        
        //필요한 경우 아래의 주석을 해제하여 depth와 normal을 사용한다.
        // Enable these if your pass requires access to the CameraDepthTexture or the CameraNormalsTexture.
        // ConfigureInput(ScriptableRenderPassInput.Depth);
        // ConfigureInput(ScriptableRenderPassInput.Normal);
        
        // Grab the color buffer from the renderer camera color target.
        // 카메라에서 렌더링된 color 정보를 저장한다.
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
        
        // Create a temporary render texture using the descriptor from above.
        //cmd.GetTemporaryRT를 통해 descriptor에 저장된 render texture를 temporaryBufferID의 버퍼와 연결지어 준다.
        cmd.GetTemporaryRT(temporaryBufferID, descriptor, FilterMode.Bilinear);
        //temporaryBufferID에 연결되어 있는 텍스쳐를 temporaryBuffer에 저장한다.
        temporaryBuffer = new RenderTargetIdentifier(temporaryBufferID);

        //함수가 끝나면 temporaryBuffer에는 직전에 렌더링 된 텍스쳐가 저장되고, colorBuffer에는 렌더링 된 컬러에 대한 정보가 저장된다.
    }

    // The actual execution of the pass. This is where custom rendering occurs.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // Grab a command buffer. We put the actual execution of the pass inside of a profiling scope.
        CommandBuffer cmd = CommandBufferPool.Get(); 
        using (new ProfilingScope(cmd, new ProfilingSampler(ProfilerTag)))
        {
            // Blit from the color buffer to a temporary buffer and back. This is needed for a two-pass shader.
            //colorBuffer에 있는 텍스쳐를 이용하여 쉐이더 연산 후, temporaryBuffer에 저장한다.
            Blit(cmd, colorBuffer, temporaryBuffer, material, 0); // shader pass 0
            Blit(cmd, temporaryBuffer, colorBuffer, material, 1); // shader pass 1
        }

        // Execute the command buffer and release it.
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    
    //render pass가 끝난 후에 한 번씩 실행된다.
    // Called when the camera has finished rendering.
    // Here we release/cleanup any allocated resources that were created by this pass.
    // Gets called for all cameras i na camera stack.
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new ArgumentNullException("cmd");
        
        // Since we created a temporary render texture in OnCameraSetup, we need to release the memory here to avoid a leak.
        // 메모리 누수를 방지하기 위해 temporaryBufferID를 해제해주어야 한다.
        cmd.ReleaseTemporaryRT(temporaryBufferID);
    }
}