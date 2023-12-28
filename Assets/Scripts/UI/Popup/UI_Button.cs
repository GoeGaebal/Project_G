using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UI_Button : UI_Popup
{
    [SerializeField] private TextMeshProUGUI _text;


    enum Buttons
    {
        PointButton
    }

    enum Texts
    {
        PointText,
        ScoreText
    }

    enum GameObjects
    {
        TestObject,
    }

    enum Images
    {
        ItemIcon,
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        
        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnClickedEvent);
        
        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        AddUIEvent(go,((PointerEventData data) => go.gameObject.transform.position = data.position), Define.UIEvent.Drag);
    }

    private int _score = 0;
    public void OnClickedEvent(PointerEventData evt)
    {
        _score++;
        Get<TextMeshProUGUI>((int)Texts.ScoreText).text = $"점수 : {_score}";
    }
}
