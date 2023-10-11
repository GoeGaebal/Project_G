// ScriptableRendererFeature template created for URP 12 and Unity 2021.2
// Made by Alexander Ameye 
// https://alexanderameye.github.io/

using UnityEngine;
using UnityEngine.Rendering.Universal;

//feature는 universial render data에 등록되며 인터페이스의 역할도 한다.
public class TemplateFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PassSettings
    {
        // Where/when the render pass should be injected during the rendering process.
        // 언제 어디서 실행될 지 정한다.
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        
        // Used for any potential down-sampling we will do in the pass.
        [Range(1,4)] public int downsample = 1;
        
        // A variable that's specific to the use case of our pass.
        [Range(0, 20)] public int blurStrength = 5;
        
        // additional properties ...
    }

    // References to our pass and its settings.
    TemplatePass pass;
    public PassSettings passSettings = new();

    // Gets called every time serialization happens.
    // Gets called when you enable/disable the renderer feature.
    // Gets called when you change a property in the inspector of the renderer feature.
    public override void Create()
    {
        // Pass the settings as a parameter to the constructor of the pass.
        pass = new TemplatePass(passSettings);
    }

    // Injects one or multiple render passes in the renderer.
    // Gets called when setting up the renderer, once per-camera.
    // Gets called every frame, once per-camera.
    // Will not be called if the renderer feature is disabled in the renderer inspector.
    // 매 프레임마다 호출되어 rener pass에 사용자가 생성한 render pass를 추가한다.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Here you can queue up multiple passes after each other.
        renderer.EnqueuePass(pass); 
    }
}