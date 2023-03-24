using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class ItemSlotUI : MonoBehaviour
    {
        public int Index { get; private set; }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetSlotIndex(int index) => Index = index;
    }
}
