using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreateRoomSetting : UI_Popup
{
    enum Buttons
    {
        CreateBtn,
        ExitBtn,
    }

    enum Texts
    {
        RoomName,
        UserName,
        WarningText,
    }

    enum GameObjects
    {
        LoadingSet,
    }
    public TextMeshProUGUI RoomName, UserName, WarningText;
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
        RoomName = GetTextMeshPro((int)Texts.RoomName);
        UserName = GetTextMeshPro((int)Texts.UserName);
        
        WarningText = GetTextMeshPro((int)Texts.WarningText);
        GetButton((int)Buttons.ExitBtn).onClick.RemoveAllListeners();
        GetButton((int)Buttons.ExitBtn).onClick.AddListener((() => { Managers.UI.ClosePopupUI(); }));
        LoadingSet = GetObject((int)GameObjects.LoadingSet);
        LoadingSet.SetActive(false);
        
        CreateRoomBtn.onClick.RemoveAllListeners();
        CreateRoomBtn.onClick.AddListener(() =>
        {
            Managers.Network.CreateRoom();
            
        });
    }
    
    public void SetInteractableButtons(bool value)
    {
        GetButton((int)Buttons.CreateBtn).interactable = value;
        GetButton((int)Buttons.ExitBtn).interactable = value;
    }
}
