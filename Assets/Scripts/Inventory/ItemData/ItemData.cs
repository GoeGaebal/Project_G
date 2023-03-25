using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory {
    public abstract class ItemData : ScriptableObject
    {
        public int ID => _id;
        public string Name => _name;
        //public string Tooltip => _tooltip;
        public Sprite IconSprite => _iconSprite;

        [SerializeField] private int _id;
        [SerializeField] private string _name;
        //[Multiline]
        //[SerializeField] private string _tooltip;
        [SerializeField] private Sprite _iconSprite;
        //[SerializeField] private GameObject _dropItemPrefab;

        public abstract Item CreateItem();
    }
}
