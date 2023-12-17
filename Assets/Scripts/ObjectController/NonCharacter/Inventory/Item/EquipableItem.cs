using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EquipableItem : Item
{
    public float HP => _hp;
    public float Damage => _damage;//공격대미지

    [SerializeField] private float _hp;
    [SerializeField] private float _damage;
    
    public virtual void ChangeEquipableItem()
    {}
    public override void Select()
    {
        base.Select();

        Managers.Network.LocalPlayer.attackDamage += Damage;
        Managers.Network.LocalPlayer.StatInfo.MaxHp += HP;
        Managers.Network.LocalPlayer.RestoreHP(Managers.Network.LocalPlayer.HP + HP);
    }
    public override void Deselect()
    {
        base.Deselect();
        Managers.Network.LocalPlayer.attackDamage -= Damage;
        Managers.Network.LocalPlayer.StatInfo.MaxHp -= HP;
        Managers.Network.LocalPlayer.RestoreHP(-HP);
    }
    public override void Skill()
    {
        base.Skill();
    }
}
