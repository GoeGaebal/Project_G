using System;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using WebSocketSharp;

// TODO : UI_Chat에서 직접 Send 하는 부분을 옮겨야 함
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
        
        foreach (Transform child in Get<GameObject>((int)GameObjects.Content).transform)
        {
            if (child != null && child.gameObject.activeSelf)
                Managers.Resource.Destroy(child.gameObject);
        }
        
        Managers.Input.UIActions.Submit.AddEvent(FocusInputField);
        Get<TMP_InputField>((int)InputFields.InputField).onSelect.AddListener(delegate { Managers.Input.PlayerActionMap.Disable(); });
        Get<TMP_InputField>((int)InputFields.InputField).onDeselect.AddListener(delegate { Managers.Input.PlayerActionMap.Enable(); });
        Managers.Network.UIChat = this;
        Get<TMP_InputField>((int)InputFields.InputField).onSubmit.AddListener(delegate { SendChat(); } );
        // Get<TMP_InputField>((int)InputFields.InputField).onEndEdit.AddListener(OnEndEditEventMethod);
    }

    private void OnDestroy()
    {
        Managers.Input.UIActions.Submit.RemoveEvent(FocusInputField);
    }

    public void FocusInputField(InputAction.CallbackContext context)
    {
        TMP_InputField inputField = Get<TMP_InputField>((int)InputFields.InputField);
        if (!inputField.isFocused)
        {
            inputField.ActivateInputField();
        }
    }

    private void SendChat()
    {
        TMP_InputField inputField = Get<TMP_InputField>((int)InputFields.InputField);
        if (inputField.text.IsNullOrEmpty()) return;
        
        C_Chat chat = new C_Chat();
        chat.Msg = String.Copy(inputField.text);
        Managers.Network.Client.Send(chat);
        inputField.text = "";
    }

    public void UpdateChat(string text)
    {
        GameObject chatText = Managers.Resource.Instantiate("UI/SubItem/UI_ChatText", Vector3.zero, Quaternion.identity,
            parent: Get<GameObject>((int)GameObjects.Content).transform);
        // Managers.UI.MakeSubItem<UI_ChatText>(parent : Get<GameObject>((int)GameObjects.Content).transform);
        chatText.GetComponent<TextMeshProUGUI>().text = text;
    }
}
