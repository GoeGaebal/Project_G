using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Application = UnityEngine.Device.Application;

public class UI_Start : UI_Scene
{
    private LobbyScene _lobbyScene;
    public TextMeshProUGUI _loadingText;
    public GameObject LoadingIcon;
    static public Animator animator;
    enum Buttons
    {
        StartBtn,
        OptionBtn,
        ExitBtn,
    }

    enum Texts
    {
        LoadingText
    }

    enum GameObjects
    {
        LoadingIcon
    }

    enum Animators
    {
        UI_Start
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Animator>(typeof(Animators));
        _lobbyScene = FindObjectOfType<LobbyScene>();
        GetButton((int)Buttons.StartBtn).onClick.RemoveAllListeners();
        animator = Get<Animator>((int)Animators.UI_Start);
        GetButton((int)Buttons.StartBtn).onClick.AddListener(() => { animator.SetTrigger("PlayButtonClicked"); });
        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent((evt) => { Application.Quit(); });
        _loadingText = GetTextMeshPro((int)Texts.LoadingText);
        LoadingIcon = GetObject((int)GameObjects.LoadingIcon);
        LoadingIcon.SetActive(false);
    }

    public void SetInteractableButtons(bool value)
    {
        GetButton((int)Buttons.StartBtn).interactable = value;
        GetButton((int)Buttons.OptionBtn).interactable = value;
        GetButton((int)Buttons.ExitBtn).interactable = value;
    }

    public void ShowUI_Lobby()
    {
        Managers.UI.ShowPopupUI<UI_Lobby>();
    }
}
