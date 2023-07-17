using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


enum GameObjects
{
        Background
}
 enum Buttons
    {
        CloseButton
    }
public class UI_PlayerStatus : UI_Scene
{
    private GameObject _backgroundUI;
    private GameObject _closeButton;
    
    private void Start() {
        Init();
    }
    public override void Init()
    {
        base.Init();
        // 전체를 관리하는 자식 오브젝트를 먼저 바인드 하고, 그 자식 오브젝트를 setActive(false)한다.
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClickCloseButton);
        Managers.Input.PlayerActions.PlayerStatus.AddEvent(OnOffPlayerStatus);

        _backgroundUI = GetObject((int)GameObjects.Background);
        _backgroundUI.SetActive(false);
    }

    public void OnOffPlayerStatus(InputAction.CallbackContext context)
    {
        if (_backgroundUI.activeSelf)//인벤토리가 켜져 있으면 
        {
            _backgroundUI.SetActive(false);//인벤토리 끔
        }
        else//인벤토리가 꺼져 있으면
        {
            _backgroundUI.SetActive(true);//인벤토리 켬
        }

    }

    public void ClickCloseButton(PointerEventData evt)
    {
        if (_backgroundUI.activeSelf)
        {
            _backgroundUI.SetActive(false);
        }
        else
        {
            _backgroundUI.SetActive(true);
        }

    }
}
