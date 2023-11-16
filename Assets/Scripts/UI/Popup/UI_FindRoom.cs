using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class UI_FindRoom : UI_Popup
{
    enum Buttons
    {
        FindBtn,
        ExitBtn,
    }

    enum TMP_InputFields
    {
        RoomAddressArea,
        RoomPortArea,
        UserNameArea,
    }

    enum Texts
    {
        WarningText
    }
    
    enum Toggles
    {
        LocalHost
    }

    enum GameObjects
    {
        LoadingSet,
    }
    private TextMeshProUGUI _warningText;
    private TMP_InputField _port, _name, _address; 
    private Button _findBtn;
    private GameObject _loadingSet;

    private bool _isConnectedFailed;
    
    private void Start()
    {
        if(_findBtn == null)
            Init();
    }

    private void Update()
    {
        if (!_isConnectedFailed) return;
        _warningText.SetText($"연결에 실패하였습니다.");
        SetInteractableButtons(true);
        _isConnectedFailed = false;
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(TMP_InputFields));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Toggle>(typeof(Toggles));
        
        _findBtn = GetButton((int)Buttons.FindBtn);
        _port = Get<TMP_InputField>((int)TMP_InputFields.RoomPortArea);
        _name = Get<TMP_InputField>((int)TMP_InputFields.UserNameArea);
        _address = Get<TMP_InputField>((int)TMP_InputFields.RoomAddressArea);
        _warningText = GetTextMeshPro((int)Texts.WarningText);
        
        GetButton((int)Buttons.ExitBtn).onClick.RemoveAllListeners();
        GetButton((int)Buttons.ExitBtn).onClick.AddListener((() => { Managers.UI.ClosePopupUI(); }));
        _loadingSet = GetObject((int)GameObjects.LoadingSet);
        _loadingSet.SetActive(false);

        var toggle = Get<Toggle>((int)Toggles.LocalHost);
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(call: value => _address.interactable = !value);
        _address.interactable = !toggle.isOn;
        
        _findBtn.onClick.RemoveAllListeners();
        _findBtn.onClick.AddListener(() =>
        {
            SetInteractableButtons(false);
            var portText = _port.text.Trim((char)8203);;
            var nameText = _name.text.Trim((char)8203);;
            var addressText = _address.text.Trim((char)8203);
            if (int.TryParse(portText, out var port) && port is >= 1024 and < 65536)
            {
                if(Get<Toggle>((int)Toggles.LocalHost).isOn) Managers.Network.Client.Connect(OnConnectedFailed, port);
                else Managers.Network.Client.Connect(addressText, OnConnectedFailed, port);
            }
            else
            {
                _warningText.SetText($"포트번호는 1024~65535 사이의 정수여야 합니다.");
                SetInteractableButtons(true);
            }
        });
        _isConnectedFailed = false;
    }
    
    private void SetInteractableButtons(bool value)
    {
        GetButton((int)Buttons.FindBtn).interactable = value;
        GetButton((int)Buttons.ExitBtn).interactable = value;
        _name.interactable = value;
        _port.interactable = value;
    }

    private void OnConnectedFailed()
    {
        _isConnectedFailed = true;
    }
}
