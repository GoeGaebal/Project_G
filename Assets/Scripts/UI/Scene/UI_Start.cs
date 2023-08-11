using System;
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
        _lobbyScene = FindObjectOfType<LobbyScene>();
        GetButton((int)Buttons.StartBtn).onClick.RemoveAllListeners();
        GetButton((int)Buttons.StartBtn).onClick.AddListener(() => { _lobbyScene.ConnectToMasterServer();});
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
}
