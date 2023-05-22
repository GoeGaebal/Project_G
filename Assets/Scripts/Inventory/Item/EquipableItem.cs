using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EquipableItem : Item
{
    /*
    public int MaxDurability => _maxDurability;//최대 내구도
    public int CurrentDurability => _currentDurability;//현재 내구도

    [SerializeField] private int _maxDurability = 200;
    [SerializeField] private int _currentDurability = _maxDurability;
    */


    //이해랑이 추가
    public virtual void ChangeEquipableItem()
    {}

}
