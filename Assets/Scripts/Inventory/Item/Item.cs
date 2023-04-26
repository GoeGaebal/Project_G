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
    /*
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);

    public bool stackable = true;

    public Sprite Icon;*/
}
/*
public enum ItemType
{
    Resource,
    Tool,
    Weapon,
    Food
}

public enum ActionType
{
    Attack,
    Dig,
    Chop,
    Eat,
    Nothing
}
*/