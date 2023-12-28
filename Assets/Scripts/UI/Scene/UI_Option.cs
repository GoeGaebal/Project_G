using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Option : UI_Popup
{
    enum GameObjects
    {
        //OptionPanel
    }
    enum Buttons
    {
        ResumeButton,
        QuitButton,
        ExitRoomButton
    }
    enum Sliders
    {
        BgmVolumeSlider,
        EffectVolumeSlider
    }
    enum Toggles
    {
        QHD,
        FHD,
        HD,
        ExclusiveFullscreen,
        //FullscreenWindow,
        //MaximizedWindow,
        Windowed
    }

    //static public GameObject optionPanel;

    public override void Init()
    {
        base.Init();

        //Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        Bind<Toggle>(typeof(Toggles));

        //optionPanel = Get<GameObject>((int)GameObjects.OptionPanel);

        CheckFullScreenMode();
        CheckResolution();

        Get<Slider>((int)Sliders.BgmVolumeSlider).value = Managers.Sound.bgmVolume;
        Get<Slider>((int)Sliders.EffectVolumeSlider).value = Managers.Sound.effectVolume;

        GetButton((int)Buttons.ResumeButton).gameObject.BindEvent(CloseOption);
        GetButton((int)Buttons.QuitButton).gameObject.BindEvent(QuitGame);
        GetButton((int)Buttons.ExitRoomButton).onClick.AddListener(ExitRoom);
        Get<Toggle>((int)Toggles.QHD).onValueChanged.AddListener(ChangeResolutionQHD);
        Get<Toggle>((int)Toggles.FHD).onValueChanged.AddListener(ChangeResolutionFHD);
        Get<Toggle>((int)Toggles.HD).onValueChanged.AddListener(ChangeResolutionHD);
        Get<Slider>((int)Sliders.BgmVolumeSlider).onValueChanged.AddListener(ChangeBgmVolume);
        Get<Slider>((int)Sliders.EffectVolumeSlider).onValueChanged.AddListener(ChangeEffectVolume);
        Get<Toggle>((int)Toggles.ExclusiveFullscreen).onValueChanged.AddListener(ChangeExclusiveFullscreen);
        //Get<Toggle>((int)Toggles.FullscreenWindow).onValueChanged.AddListener(ChangeFullscreenWindow);
        //Get<Toggle>((int)Toggles.MaximizedWindow).onValueChanged.AddListener(ChangeMaximizedWindow);
        Get<Toggle>((int)Toggles.Windowed).onValueChanged.AddListener(ChangeWindowed);
    }

    private void CloseOption(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI();
        Managers.Option.isOptionPanelOn = false;
        Managers.Input.PlayerActionMap.Enable();
    }

    private void QuitGame(PointerEventData evt)
    {
        Application.Quit();
    }
    
    private void ExitRoom()
    {
        if (Managers.Network.IsHost) Managers.Network.Server.ShutDown();
        else Managers.Network.Client.DisConnect();
    }

    public void CheckResolution()//모니터 해상도 체크해서 설정 적용
    {
        switch (Managers.Option.resolution) {
            case 0:
                Get<Toggle>((int)Toggles.QHD).isOn = true;
                break;
            case 1:
                Get<Toggle>((int)Toggles.FHD).isOn = true;
                break;
            case 2:
                Get<Toggle>((int)Toggles.HD).isOn = true;
                break;
            default:
                Debug.Log("Resolution is not QHD, FHD or HD");
                Get<Toggle>((int)Toggles.HD).isOn = true;
                ChangeResolutionHD(Get<Toggle>((int)Toggles.HD).isOn);
                break;
        }
    }

    public void ChangeResolutionQHD(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(2560, 1440, true);
            Managers.Option.resolution = 0;
            //Debug.Log("QHD");
        }
    }

    public void ChangeResolutionFHD(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(1920, 1080, true);
            Managers.Option.resolution = 1;
            //Debug.Log("FHD");
        }
    }

    public void ChangeResolutionHD(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(1280, 720, true);
            Managers.Option.resolution = 2;
            //Debug.Log("HD");
        }
    }

    public void CheckFullScreenMode()//초기 화면 모드 체크
    {
        switch (Managers.Option.screen)
        {
            case 0:
                Get<Toggle>((int)Toggles.Windowed).isOn = true;
                break;
            default:
                Get<Toggle>((int)Toggles.ExclusiveFullscreen).isOn = true;
                break;
        }
    }

    public void ChangeExclusiveFullscreen(bool isOn)//전체화면
    {
        if (isOn)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Managers.Option.screen = 1;
        }
    }
    /*
    public void ChangeFullscreenWindow(bool isOn)
    {
        if (isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
    }

    public void ChangeMaximizedWindow(bool isOn)
    {
        if (isOn)
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        }
    }
    */
    public void ChangeWindowed(bool isOn)//창모드
    {
        if (isOn)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Managers.Option.screen = 0;
        }
    }

    public void ChangeBgmVolume(float volume)
    {
        Managers.Sound.ChangeBGMVolume(volume);
    }

    public void ChangeWeatherVolume(float volume)
    {
        Managers.Sound.ChangeWeatherVolume(volume);
    }

    public void ChangeEffectVolume(float volume)
    {
        Managers.Sound.ChangeEffectVolume(volume);
    }
}
