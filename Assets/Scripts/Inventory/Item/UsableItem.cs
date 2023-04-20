using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : CountableItem
{
    public abstract void UseItem();//음식 먹기, 포션 섭취 등 아이템 사용
}
