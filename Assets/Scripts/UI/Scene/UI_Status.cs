using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Status : UI_Scene
{
    enum GameObjects
    {
        Timer,
        OptionPanel,
        OtherStatus
    }
    enum Images { HPBar }
    enum Texts { HPText }
    enum Buttons 
    { 
        OptionButton,
        ResumeButton
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

    private Image hpBar;
    private TextMeshProUGUI hpText;

    private GameObject[] players;
    private GameObject playerGO;

    private GameObject rotatingTimer;
    private float ratio = 0;

    private GameObject _optionPanel;

    private UI_OtherHP[] uiOtherHps;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        Bind<Toggle>(typeof(Toggles));

        hpBar = GetImage((int)Images.HPBar);
        hpText = GetTextMeshPro((int)Texts.HPText);

        rotatingTimer = GetObject((int)GameObjects.Timer);

        _optionPanel = Get<GameObject>((int)GameObjects.OptionPanel);
        _optionPanel.SetActive(false);

        CheckFullScreenMode();
        CheckResolution();

        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OpenOption);
        GetButton((int)Buttons.ResumeButton).gameObject.BindEvent(CloseOption);
        Get<Toggle>((int)Toggles.QHD).onValueChanged.AddListener(ChangeResolutionQHD);
        Get<Toggle>((int)Toggles.FHD).onValueChanged.AddListener(ChangeResolutionFHD);
        Get<Toggle>((int)Toggles.HD).onValueChanged.AddListener(ChangeResolutionHD);
        Get<Slider>((int)Sliders.BgmVolumeSlider).onValueChanged.AddListener(ChangeBgmVolume);
        Get<Slider>((int)Sliders.EffectVolumeSlider).onValueChanged.AddListener(ChangeEffectVolume);
        Get<Toggle>((int)Toggles.ExclusiveFullscreen).onValueChanged.AddListener(ChangeExclusiveFullscreen);
        //Get<Toggle>((int)Toggles.FullscreenWindow).onValueChanged.AddListener(ChangeFullscreenWindow);
        //Get<Toggle>((int)Toggles.MaximizedWindow).onValueChanged.AddListener(ChangeMaximizedWindow);
        Get<Toggle>((int)Toggles.Windowed).onValueChanged.AddListener(ChangeWindowed);

        uiOtherHps = new[]
        {
            Managers.UI.MakeSubItem<UI_OtherHP>(parent: GetObject((int)GameObjects.OtherStatus).transform),
            Managers.UI.MakeSubItem<UI_OtherHP>(parent: GetObject((int)GameObjects.OtherStatus).transform)
        };
        foreach(var ui in uiOtherHps)
            ui.Init();
    }
    
    private void Update()
    {
        UpdateHPBar();
        UpdateHPText();
        int i = 0;
        foreach (var player in Managers.Object.OtherPlayerDict.Values) uiOtherHps[i++].SetStatus(player);
    }

    void FixedUpdate()
    {
        AddTime();
    }

    private void UpdateHPBar()
    {
        if (Managers.Network.LocalPlayer != null)
        {
            float temp = Managers.Network.LocalPlayer.HP / Managers.Network.LocalPlayer.maxHP;
            hpBar.fillAmount = temp;
        }
    }

    private void UpdateHPText()
    {
        if(Managers.Network.LocalPlayer != null)
        {
            hpText.SetText(Managers.Network.LocalPlayer.HP + " / " + Managers.Network.LocalPlayer.maxHP);
        }
    }
    
    private void AddTime()
    {
        rotatingTimer.transform.rotation *= Quaternion.Euler(0f, 0f, 180f * ratio);
    }

    private void OpenOption(PointerEventData evt)
    {
        _optionPanel.SetActive(true);
        Managers.Input.PlayerActionMap.Disable();
    }

    private void CloseOption(PointerEventData evt)
    {
        _optionPanel.SetActive(false);
        Managers.Input.PlayerActionMap.Enable();
    }

    public void CheckResolution()//모니터 해상도 체크해서 설정 적용
    {
        int width = Screen.currentResolution.width;
        int height = Screen.currentResolution.height;

        if (width == 2560 && height == 1440)
        {
            //Debug.Log("Resolution is QHD");
            Get<Toggle>((int)Toggles.QHD).isOn = true;
        }
        else if (width == 1920 && height == 1080)
        {
            //Debug.Log("Resolution is FHD");
            Get<Toggle>((int)Toggles.FHD).isOn = true;
        }
        else if (width == 1280 && height == 720)
        {
            //Debug.Log("Resolution is HD");
            Get<Toggle>((int)Toggles.HD).isOn = true;
        }
        else
        {
            Debug.Log("Resolution is not QHD, FHD or HD");
            Get<Toggle>((int)Toggles.HD).isOn = true;
            ChangeResolutionHD(Get<Toggle>((int)Toggles.HD).isOn);
        }
    }

    public void ChangeResolutionQHD(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(2560, 1440, true);
            //Debug.Log("QHD");
        }
    }

    public void ChangeResolutionFHD(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(1920, 1080, true);
            //Debug.Log("FHD");
        }
    }

    public void ChangeResolutionHD(bool isOn)
    {
        if (isOn)
        {
            Screen.SetResolution(1280, 720, true);
            //Debug.Log("HD");
        }
    }

    public void CheckFullScreenMode()//초기 화면 모드 체크
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            Get<Toggle>((int)Toggles.Windowed).isOn = true;
        }
        else
        {
            Get<Toggle>((int)Toggles.ExclusiveFullscreen).isOn = true;
        }
    }

    public void ChangeExclusiveFullscreen(bool isOn)//전체화면
    {
        if (isOn)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
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
