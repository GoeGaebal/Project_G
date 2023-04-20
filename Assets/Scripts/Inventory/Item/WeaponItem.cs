using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/WeaponItem")]
public class WeaponItem : EquipableItem
{
    public float AttackDamge => _attackDamage;//공격대미지
    public float AttackSpeed => _attackSpeed;//공격속도

    [SerializeField] private float _attackDamage = 1f;
    [SerializeField] private float _attackSpeed = 1f;
}
