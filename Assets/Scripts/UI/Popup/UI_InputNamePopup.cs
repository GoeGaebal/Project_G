using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InputNamePopup : UI_Popup
{
    enum Buttons
    {
        EnterBtn,
        ExitBtn,
    }

    enum Texts
    {
        UserName,
        WarningText,
    }

    enum GameObjects
    {
        LoadingPane,
    }

    public GameObject LoadingPane;
    public TextMeshProUGUI UserName, WarningText;
    public Button EnterBtn;
    
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        EnterBtn = GetButton((int)Buttons.EnterBtn);
        UserName = GetTextMeshPro((int)Texts.UserName);
        LoadingPane = GetObject((int)GameObjects.LoadingPane);
        
        WarningText = GetTextMeshPro((int)Texts.WarningText);
        GetButton((int)Buttons.ExitBtn).onClick.RemoveAllListeners();
        GetButton((int)Buttons.ExitBtn).onClick.AddListener((() => { Managers.UI.ClosePopupUI(); }));
        LoadingPane.SetActive(false);
    }
    
    public void SetInteractableButtons(bool value)
    {
        GetButton((int)Buttons.EnterBtn).interactable = value;
        GetButton((int)Buttons.ExitBtn).interactable = value;
    }
}
