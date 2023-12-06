using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SystemMessage : UI_Scene
{
    enum Images
    {
        Background,
    }

    enum TMPs
    {
        MessageTMP,
    }

    private Image _background;
    private TMP_Text _MessageText;

    public static System.Action<string, Color> alert;

    private void Awake()
    {
        alert = (t, c) => { Alert(t, c); };
    }

    void Start()
    {
        Init();
    }


    void Update()
    {

    }

    public override void Init()
    {
        //base.Init();
        Managers.UI.SetCanvas(gameObject, true);

        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(TMPs));

        _background = Get<Image>((int)Images.Background);
        _background.gameObject.SetActive(false);
        _MessageText = Get<TMP_Text>((int)TMPs.MessageTMP);
        _MessageText.gameObject.SetActive(false);
    }

    public void Alert(string text, Color color)
    {
        OpenUI();
        _background.color = color;
        SetText(text);
        Invoke("CloseUI", 1.5f);
    }
    
    public void SetText(string text)
    {
        _MessageText.text = text;
    }

    public void OpenUI()
    {
        _background.gameObject.SetActive(true);
        _MessageText.gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        _background.gameObject.SetActive(false);
        _MessageText.gameObject.SetActive(false);
    }
}