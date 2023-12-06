using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public int ID => _id;
    public string Name => _name;
    public string Tooltip => _tooltip;
    public Sprite Icon => _icon;

    [SerializeField] private int _id;
    [SerializeField] private string _name = "";
    [Multiline]
    [SerializeField] private string _tooltip = "";
    [SerializeField] private Sprite _icon;
    [SerializeField] GameObject _droppedItemPrefab;

    public virtual void Select()
    {
        Debug.Log("아이템 " + Name + " 장착됨");
    }

    public virtual void Deselect()
    {
        Debug.Log("아이템 " + Name + " 해제됨");
    }

    public virtual void Skill()
    {

    }
}
