using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class UI_CreateRoomSetting : UI_Popup
{
    enum Buttons
    {
        CreateBtn,
        ExitBtn,
    }

    enum Texts
    {
        RoomPort,
        UserName,
        WarningText,
    }

    enum GameObjects
    {
        LoadingSet,
    }
    public TextMeshProUGUI RoomPort, UserName, WarningText;
    public Button CreateRoomBtn;
    public GameObject LoadingSet;
    
    private void Start()
    {
        if(CreateRoomBtn == null)
            Init();
    }

    
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        
        CreateRoomBtn = GetButton((int)Buttons.CreateBtn);
        RoomPort = GetTextMeshPro((int)Texts.RoomPort);
        UserName = GetTextMeshPro((int)Texts.UserName);
        
        WarningText = GetTextMeshPro((int)Texts.WarningText);
        GetButton((int)Buttons.ExitBtn).onClick.RemoveAllListeners();
        GetButton((int)Buttons.ExitBtn).onClick.AddListener((() => { Managers.UI.ClosePopupUI(); }));
        LoadingSet = GetObject((int)GameObjects.LoadingSet);
        LoadingSet.SetActive(false);
        
        CreateRoomBtn.onClick.RemoveAllListeners();
        CreateRoomBtn.onClick.AddListener(() =>
        {
            SetInteractableButtons(false);
            var portText = RoomPort.text.Trim((char)8203);;
            if(portText.IsNullOrEmpty()) Managers.Network.CreateRoom(OnConnectedFailed);
            else if (int.TryParse(portText, out var port) && port is >= 1024 and < 65536)
            {
                Managers.Network.CreateRoom(OnConnectedFailed,port);
            }
            else
            {
                WarningText.SetText($"포트번호는 1024~65535 사이의 정수여야 합니다.");
                SetInteractableButtons(true);
            }
        });
    }

    private void OnConnectedFailed()
    {
        WarningText.SetText($"방 생성에 실패하였습니다.");
        SetInteractableButtons(true);
    }

    private void SetInteractableButtons(bool value)
    {
        GetButton((int)Buttons.CreateBtn).interactable = value;
        GetButton((int)Buttons.ExitBtn).interactable = value;
    }
}
