using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PopupNameItem : UI_Base
{
    enum Texts
    {
        Name,
    }

    public TextMeshProUGUI Name;
    public Transform target;

    public override void Init()
    {
        if (Name == null)
        {
            Bind<TextMeshProUGUI>(typeof(Texts));
            Name = GetTextMeshPro((int)Texts.Name);
        }
    }

    private void LateUpdate()
    {
        if (target != null) Name.transform.position = Camera.main.WorldToScreenPoint(target.transform.position + new Vector3(0f,0.5f,0f));
    }
}
