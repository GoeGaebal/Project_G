using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OptionManager
{
    [HideInInspector] public int resolution;
    [HideInInspector] public int screen;
    [HideInInspector] public bool isOptionPanelOn;
    public void Init()
    {
        GameObject root = GameObject.Find("@Option");
        if (root == null)
        {
            root = new GameObject { name = "@Option" };
            UnityEngine.Object.DontDestroyOnLoad(root);
        }
        resolution = CheckResolution();
        screen = CheckFullScreenMode();
        isOptionPanelOn = false;

        Managers.Input.UIActions.Option.AddEvent(OnOffOption);
    }

    private void OnOffOption(InputAction.CallbackContext evt)
    {
        if (isOptionPanelOn)
        {
            Managers.UI.ClosePopupUI();
            Managers.Input.PlayerActionMap.Enable();
        }
        else
        {
            Managers.UI.ShowPopupUI<UI_Option>();
            Managers.Input.PlayerActionMap.Disable();
        }
        isOptionPanelOn = !isOptionPanelOn;
    }

    public int CheckResolution()//모니터 해상도 체크해서 설정 적용
    {
        int width = Screen.currentResolution.width;
        int height = Screen.currentResolution.height;

        if (width == 2560 && height == 1440)
        {
            //QHD;
            return 0;
        }
        else if (width == 1920 && height == 1080)
        {
            //FHD;
            return 1;
        }
        else if (width == 1280 && height == 720)
        {
            //HD;
            return 2;
        }
        else
        {
            //QHD, FHD, HD 전부 아님;
            return -1;
        }
    }

    public int CheckFullScreenMode()//초기 화면 모드 체크
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            //창모드
            return 0;
        }
        else
        {
            //전체화면
            return 1;
        }
    }
}
