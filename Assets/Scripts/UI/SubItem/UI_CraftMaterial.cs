using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_CraftMaterial : UI_Base
{
    enum GameObjects
    {
        Count,
    }

    enum Texts
    {
        Count,
    }

    private Image _icon;
    private GameObject _countGo;
    private TMP_Text _count;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TMP_Text>(typeof(Texts));

        _icon = GetComponent<Image>();
        _count = Get<TMP_Text>((int)Texts.Count);
        _countGo = Get<GameObject>((int)GameObjects.Count);
        _countGo.SetActive(false);
    }

    public void SetIcon(Sprite sprite)
    {
        _icon.sprite = sprite;
    }

    public void SetCount(int n)
    {
        _count.text = n.ToString();
        if(n > 0)
        {
            _countGo.SetActive(true);
        }
    }
}
