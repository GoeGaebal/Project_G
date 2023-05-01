using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountableItem : Item
{
    /*
    public CountableItem()
    {
        Count = _count;
    }
    */
    //public int Count { get; set; }
    public int MaxCount => _maxCount;//최대 개수

    //[SerializeField] private int _count = 1;
    [SerializeField] private int _maxCount = 3;
    
}
