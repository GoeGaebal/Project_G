using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using WebSocketSharp;

public class UI_Chat : UI_Scene
{
    enum GameObjects
    {
        Content
    }
    enum InputFields
    {
        InputField
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<TMP_InputField>(typeof(InputFields));
        Bind<GameObject>(typeof(GameObjects));
        Managers.Input.UIActions.Submit.AddEvent(FocusInputField);
        Get<TMP_InputField>((int)InputFields.InputField).onSelect.AddListener(delegate { Managers.Input.PlayerActionMap.Disable(); });
        Get<TMP_InputField>((int)InputFields.InputField).onDeselect.AddListener(delegate { Managers.Input.PlayerActionMap.Enable(); });
        Get<TMP_InputField>((int)InputFields.InputField).onSubmit.AddListener(delegate { UpdateChat(); } );
        // Get<TMP_InputField>((int)InputFields.InputField).onEndEdit.AddListener(OnEndEditEventMethod);
    }

    public void FocusInputField(InputAction.CallbackContext context)
    {
        TMP_InputField inputField = Get<TMP_InputField>((int)InputFields.InputField);
        if (!inputField.isFocused)
        {
            inputField.ActivateInputField();
        }
    }

    private void UpdateChat()
    {
        TMP_InputField inputField = Get<TMP_InputField>((int)InputFields.InputField);
        if (inputField.text == "") return;
        
        GameObject chatText = Managers.Resource.Instantiate("UI/SubItem/UI_ChatText", Vector3.zero, Quaternion.identity,
            parent: Get<GameObject>((int)GameObjects.Content).transform);
        // Managers.UI.MakeSubItem<UI_ChatText>(parent : Get<GameObject>((int)GameObjects.Content).transform);

        chatText.GetComponent<TextMeshProUGUI>().text = inputField.text;
        inputField.text = "";
    }
}
