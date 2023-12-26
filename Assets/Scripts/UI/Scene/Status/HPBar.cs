using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private Image hpBar;
    
    private GameObject[] players;
    private GameObject playerGO;

    private void Start()
    {
        hpBar = GetComponent<Image>();
    }
    
    private void Update()
    {
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (Managers.Network.LocalPlayer != null)
        {
            hpBar.fillAmount = Managers.Network.LocalPlayer.HP / Managers.Network.LocalPlayer.maxHP;
        }

    }
}
