using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory {
    [CreateAssetMenu(fileName = "Weapon Item", menuName = "Inventory/Weapon", order = int.MaxValue)]
    public class WeaponItemData : EquipmentItemData
    {
        public float Damage => _damage;
        [SerializeField] private float _damage = 1f;
        
        public override Item CreateItem()
        {
            return new WeaponItem(this);
        }
    }
}
