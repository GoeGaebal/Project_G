using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class EquipmentItem : Item
    {
        public EquipmentItemData EquipmentData { get; private set; }

        public EquipmentItem(EquipmentItemData data) : base(data)
        {
            EquipmentData = data;
        }
    }
}
