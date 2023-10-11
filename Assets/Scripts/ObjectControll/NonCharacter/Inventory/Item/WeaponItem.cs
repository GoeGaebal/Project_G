using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/WeaponItem")]
public class WeaponItem : EquipableItem
{
    public float AttackSpeed => _attackSpeed;//공격속도

    [SerializeField] private float _attackSpeed = 1f;

    public override void ChangeEquipableItem()
    {
        PlayerAttackController.ChangeWeapon(EnumWeaponList.Sword);
    }
}
