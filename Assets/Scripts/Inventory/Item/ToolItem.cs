using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ToolItem")]
public class ToolItem : EquipableItem
{
    public float Efficiency => _efficiency;//자원 캐는 속도

    [SerializeField] private float _efficiency = 1f;

    public override void ChangeEquipableItem()
    {
        PlayerAttackController.ChangeWeapon(EnumWeaponList.Axe);
    }
}
