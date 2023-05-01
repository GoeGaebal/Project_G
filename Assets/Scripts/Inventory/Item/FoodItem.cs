using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/FoodItem")]
public class FoodItem : UsableItem
{
    public float HealAmount => _healAmount;

    [SerializeField] private float _healAmount = 0;

    public override void UseItem()//음식 먹기
    {
        throw new System.NotImplementedException();
    }
}
