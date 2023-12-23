using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OtherHP : UI_Base
{
    private enum Images
    {
        HpBar
    }

    private enum Texts
    {
        HpText, PingText
    }
    
    private Image _hpBar;
    private TextMeshProUGUI _hpText, _pingText;
    
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _hpBar = GetImage((int)Images.HpBar);
        _hpText = GetTextMeshPro((int)Texts.HpText);
        _pingText = GetTextMeshPro((int)Texts.PingText);
        
        InitStatus();
        transform.localScale = Vector3.one;
    }

    private void InitStatus()
    {
        _hpBar.fillAmount = 0;
        _hpText.SetText(0 + " / " + 0);
    }
    
    public void SetStatus(Player player)
    {
        var temp = player.HP / player.maxHP;
        _hpBar.fillAmount = temp;
        _hpText.SetText(player.HP + " / " + player.maxHP);
    }
    
}
