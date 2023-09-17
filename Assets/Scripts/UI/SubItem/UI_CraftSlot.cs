using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_Base, IPointerClickHandler
{
    enum GameObjects
    {
        Count,
    }

    enum Images
    {
        Icon,
    }

    enum Texts
    {
        Count,
        Name,
        Explanation,
    }

    private int _targetID = -1;
    private Image _icon;
    private TMP_Text _count;
    private GameObject _countGo;
    private TMP_Text _name;
    private TMP_Text _explanation;

    void Start()
    {
        //Init();
    }

    void Update()
    {
        
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        
        _icon = Get<Image>((int)Images.Icon);
        _count = Get<TMP_Text>((int)Texts.Count);
        _countGo = Get<GameObject>((int)GameObjects.Count);
        _countGo.SetActive(false);
        _name = Get<TMP_Text>((int)Texts.Name);
        _explanation = Get<TMP_Text>((int)Texts.Explanation);
    }

    public void SetSlot(int id, Sprite icon, int count, string name, string explanation)
    {
        _targetID = id;
        _icon.sprite = icon;
        _count.text = count.ToString();
        if(count > 1)
        {
            _countGo.SetActive(true);
        }
        _name.text = name;
        _explanation.text = explanation;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UI_Craft.open(_targetID);
    }
}
