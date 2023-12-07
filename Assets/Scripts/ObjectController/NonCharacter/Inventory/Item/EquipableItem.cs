using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EquipableItem : Item
{
    /*
    public int MaxDurability => _maxDurability;//최대 내구도
    public int CurrentDurability => _currentDurability;//현재 내구도

    [SerializeField] private int _maxDurability = 200;
    [SerializeField] private int _currentDurability = _maxDurability;
    */
    //이해랑이 추가

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
