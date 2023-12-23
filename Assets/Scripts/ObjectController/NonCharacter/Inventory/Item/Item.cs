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

    public virtual void Init(int id, string name, string tooltip, string iconPath)
    {
        _id = id;
        _name = name;
        _tooltip = tooltip;
        _icon = Managers.Resource.Load<Sprite>(iconPath);
    }

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
