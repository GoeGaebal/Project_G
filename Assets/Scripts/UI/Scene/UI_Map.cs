using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Map : UI_Scene
{
    // TODO: 카메라 의 Cull Mask 기능을 이용하여 플레이어나 기타 프리팹에서 Marker들만 보이도록 설정함
    // 프리팹 상에서 마커가 평소에는 잘 안보이게끔 에디터 설정하거나 마커도 여기서 관리하게끔 바꾸는게 좋을 것 같음
    enum Cameras
    {
        MinimapCam
    }

    enum GameObjects
    {
        MapPanel
    }

    enum RawImages
    {
        Minimap
    }

    enum Buttons
    {
        MinimapBtn
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<RawImage>(typeof(RawImages));
        Bind<Button>(typeof(Buttons));
        Bind<Camera>(typeof(Cameras));
        
        GetButton((int)Buttons.MinimapBtn).gameObject.BindEvent(ClickBtn);
        Managers.Input.PlayerActions.MiniMap.AddEvent(PushShortKey);
        
        GetObject((int)GameObjects.MapPanel).SetActive(false);
    }

    private void FixedUpdate()
    {
        Get<Camera>((int)Cameras.MinimapCam).transform.position = Camera.main.transform.position;
    }

    private void ClickBtn(PointerEventData evt)
    {
        OnOffMinimap();
    }

    private void PushShortKey(InputAction.CallbackContext context)
    {
        OnOffMinimap();
    }

    private void OnOffMinimap()
    {
        GameObject go = GetObject((int)GameObjects.MapPanel);
        if (go.activeSelf)
            go.SetActive(false);
        else
            go.SetActive(true);
    }
}