using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HP : MonoBehaviour
{
    private Image hpBar;
    private TextMeshPro hpText;

    private GameObject[] players;
    private GameObject playerGO;

    private void Start()
    {
        hpBar = gameObject.transform.GetChild(0).GetComponent<Image>();
        hpText = gameObject.transform.GetChild(1).GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateHPBar();
        UpdateHPText();
    }

    private void UpdateHPBar()
    {
        if (Managers.Network.LocalPlayer != null)
        {
            float temp = Managers.Network.LocalPlayer.HP / Managers.Network.LocalPlayer.maxHP;
            hpBar.fillAmount = temp;
        }
        
    }

    private void UpdateHPText()
    {
        if(Managers.Network.LocalPlayer != null)
        {
            hpText.SetText(Managers.Network.LocalPlayer.HP + " / " + Managers.Network.LocalPlayer.maxHP);
        }
        
    }
}
