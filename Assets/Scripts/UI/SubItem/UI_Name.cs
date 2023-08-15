using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Name : UI_Base
{
    enum Texts
    {
        Name,
    }

    public TextMeshProUGUI Name;
    public Player target;

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Name = GetTextMeshPro((int)Texts.Name);
    }

    private void Update()
    {
        if (target != null) Name.transform.position = Camera.main.WorldToScreenPoint(target.transform.position);
    }
}
