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

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        Name = GetTextMeshPro((int)Texts.Name);
    }
}
