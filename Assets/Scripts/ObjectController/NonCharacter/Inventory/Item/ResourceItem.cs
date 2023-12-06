using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ResourceItem")]
public class ResourceItem : CountableItem
{
    public override void Select()
    {
        base.Select();
    }

    public override void Deselect()
    {
        base.Deselect();
    }

    public override void Skill()
    {
        base.Skill();
    }
}
