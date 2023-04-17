using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable Object/Item")]
public class Item : ScriptableObject
{
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);

    public bool stackable = true;

    public Sprite Icon;
}

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
