using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Friend : UI_Base
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
