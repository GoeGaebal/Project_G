using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ArmorItem")]
public class ArmorItem : EquipableItem
{
    public int Damage => _damage;
    public int Hp => _hp;

    [SerializeField] private int _damage = 0;
    [SerializeField] private int _hp = 0;
}
