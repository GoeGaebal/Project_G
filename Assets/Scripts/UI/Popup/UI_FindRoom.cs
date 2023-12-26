using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_FindRoom : UI_Popup
{
    private enum Buttons
    {
        FindBtn,
        ExitBtn,
    }

    private enum TMPInputFields
    {
        RoomAddressArea,
        RoomPortArea,
        UserNameArea,
    }

    private enum Texts
    {
        WarningText
    }

    private enum Toggles
    {
        LocalHost
    }

    private enum GameObjects
    {
        LoadingSet,
    }
    private TextMeshProUGUI _warningText;
    private TMP_InputField _port, _name, _address; 
    private Button _findBtn;
    private GameObject _loadingSet;

    private string _nameText;

    private bool _isConnectedFailed;
    private bool _isConnectedSucceed;
    
    private void Start()
    {
        if(_findBtn == null)
            Init();
    }

    private void Update()
    {
        if (_isConnectedFailed)
        {
            _warningText.SetText($"연결에 실패하였습니다.");
            SetInteractableButtons(true);
            _isConnectedFailed = false;
        }

        if (!_isConnectedSucceed) return;
        
        Managers.UI.Clear();
        Managers.UI.SetEventSystem();
        Managers.UI.ShowSceneUI<UI_Inven>();
        //Managers.UI.ShowSceneUI<UI_Map>();
        Managers.UI.ShowSceneUI<UI_Status>();
        Managers.UI.ShowSceneUI<UI_Chat>();
        Managers.UI.ShowSceneUI<UI_Leaf>();
        Managers.Map.LoadMap(5);
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(TMPInputFields));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Toggle>(typeof(Toggles));
        
        _findBtn = GetButton((int)Buttons.FindBtn);
        _port = Get<TMP_InputField>((int)TMPInputFields.RoomPortArea);
        _name = Get<TMP_InputField>((int)TMPInputFields.UserNameArea);
        _address = Get<TMP_InputField>((int)TMPInputFields.RoomAddressArea);
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
            _nameText = _name.text.Trim((char)8203);;
            var addressText = _address.text.Trim((char)8203);
            if (int.TryParse(portText, out var port) && port is >= 1024 and < 65536)
            {
                if (!string.IsNullOrEmpty(_nameText) && _nameText.Length <= 6)
                {
                    if(Get<Toggle>((int)Toggles.LocalHost).isOn) Managers.Network.Client.Connect(OnConnectedSucceed, OnConnectedFailed, port);
                    else Managers.Network.Client.Connect(addressText, OnConnectedSucceed, OnConnectedFailed, port);
                }
                else
                {
                    _warningText.SetText($"이름은 비어있지 않거나 6글자 이내여야 합니다.");
                    SetInteractableButtons(true);
                }
            }
            else
            {
                _warningText.SetText($"포트번호는 1024~65535 사이의 정수여야 합니다.");
                SetInteractableButtons(true);
            }
        });
        _isConnectedFailed = false;
        _isConnectedSucceed = false;
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
    
    private void OnConnectedSucceed()
    {
        Managers.Network.UserName = _nameText;
        _isConnectedSucceed = true;
    }
}
