
using System;
using TMPro;
using UnityEngine.UI;

public class UI_OtherHP : UI_Base
{
    private enum Images
    {
        HPBar
    }

    private enum Texts
    {
        HPText
    }

    public Player player;
    private Image _hpBar;
    private TextMeshProUGUI _hpText;
    
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _hpBar = GetImage((int)Images.HPBar);
        _hpText = GetTextMeshPro((int)Texts.HPText);
    }

    public void Update()
    {
        if (player != null)
        {
            float temp = player.HP / player.maxHP;
            _hpBar.fillAmount = temp;
            _hpText.SetText(player.HP + " / " + player.maxHP);
        }
    }
    
}
