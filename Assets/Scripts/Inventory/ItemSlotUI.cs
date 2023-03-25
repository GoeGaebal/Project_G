using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class ItemSlotUI : MonoBehaviour
    {
        public int Index { get; private set; }

        public void SetSlotIndex(int index) => Index = index;
    }
}
