using UnityEngine;
using TMPro;

public class HPText : MonoBehaviour
{
    private TMP_Text hpText;

    private void Start()
    {
        hpText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        UpdateHPText();
    }

    private void UpdateHPText()
    {
        if (Managers.Network.LocalPlayer != null)
        {
            hpText.text = Managers.Network.LocalPlayer.HP + " / " + Managers.Network.LocalPlayer.maxHP;
        }

    }
}
