using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/WeaponItem")]
public class WeaponItem : EquipableItem
{
    public float AttackSpeed => _attackSpeed;//공격속도

    [SerializeField] private float _attackSpeed = 1f;
    
    public virtual void Init(int id, string name, string tooltip, string iconPath, float hp, float damage, float attackSpeed)
    {
        base.Init(id,name,tooltip,iconPath, hp, damage);
        _attackSpeed = attackSpeed;
    }

    public override void Select()
    {
        base.Select();
        Managers.Network.LocalPlayer.EquipWeapon(ID);
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
