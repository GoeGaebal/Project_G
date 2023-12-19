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
    public TextMeshProUGUI roomPort, userName, warningText;
    public Button createRoomBtn;
    public GameObject loadingSet;
    
    private bool _isConnectedFailed;
    private bool _isConnectedSucceed;
    
    private void Start()
    {
        if(createRoomBtn == null)
            Init();
    }

    private void Update()
    {
        if (_isConnectedFailed)
        {
            warningText.SetText($"연결에 실패하였습니다.");
            SetInteractableButtons(true);
            _isConnectedFailed = false;
        }
        if (_isConnectedSucceed)
        {
            Managers.UI.Clear();
            Managers.UI.SetEventSystem();
            Managers.UI.ShowSceneUI<UI_Inven>();
            //Managers.UI.ShowSceneUI<UI_Map>();
            Managers.UI.ShowSceneUI<UI_Status>();
            Managers.UI.ShowSceneUI<UI_Chat>();
            Managers.UI.ShowSceneUI<UI_Leaf>();
            Managers.UI.ShowSceneUI<UI_Crosshair>();
            Managers.Map.LoadMap(5);
        }
    }

    
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        
        createRoomBtn = GetButton((int)Buttons.CreateBtn);
        roomPort = GetTextMeshPro((int)Texts.RoomPort);
        userName = GetTextMeshPro((int)Texts.UserName);
        
        warningText = GetTextMeshPro((int)Texts.WarningText);
        GetButton((int)Buttons.ExitBtn).onClick.RemoveAllListeners();
        GetButton((int)Buttons.ExitBtn).onClick.AddListener((() => { Managers.UI.ClosePopupUI(); }));
        loadingSet = GetObject((int)GameObjects.LoadingSet);
        loadingSet.SetActive(false);

        roomPort.text = "7777";
        createRoomBtn.onClick.RemoveAllListeners();
        createRoomBtn.onClick.AddListener(() =>
        {
            SetInteractableButtons(false);
            var portText = roomPort.text.Trim((char)8203);;
            var networkUserName = userName.text.Trim((char)8203);
            if(portText.IsNullOrEmpty()) Managers.Network.CreateRoom(() =>
            {
                Managers.Network.UserName = networkUserName;
                _isConnectedSucceed = true;
            }, () => _isConnectedFailed = true);
            else if (int.TryParse(portText, out var port) && port is >= 1024 and < 65536)
            {
                if (!networkUserName.IsNullOrEmpty() && networkUserName.Length <= 6)
                {
                    Managers.Network.CreateRoom(() =>
                    {
                        Managers.Network.UserName = networkUserName;
                        _isConnectedSucceed = true;
                    }, () => _isConnectedFailed = true ,port);
                }
                else
                {
                    warningText.SetText($"이름은 비어있지 않거나 6글자 이내여야 합니다.");
                    SetInteractableButtons(true);
                }
            }
            else
            {
                warningText.SetText($"포트번호는 1024~65535 사이의 정수여야 합니다.");
                SetInteractableButtons(true);
            }
        });
        
        _isConnectedFailed = false;
        _isConnectedSucceed = false;
    }

    private void SetInteractableButtons(bool value)
    {
        GetButton((int)Buttons.CreateBtn).interactable = value;
        GetButton((int)Buttons.ExitBtn).interactable = value;
    }
}
