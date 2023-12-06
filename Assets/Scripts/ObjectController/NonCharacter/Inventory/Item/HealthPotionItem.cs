using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/HealthPotionItem")]
public class HealthPotionItem : UsableItem
{
    public int recoveryAmount = 0;
    public void UsePotion(UI_Item selectedSlot)
    {
        //체력 회복하는 코드
        base.UseItem(selectedSlot);
    }

    public override void Select()
    {
        base.Select();
    }

    public override void Deselect()
    {
        base.Deselect();
    }

    public override void Skill()
    {
        base.Skill();
    }
}
