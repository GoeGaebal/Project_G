using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountableItem : Item
{
    public CountableItem()
    {
        Count = _count;
    }
    public int MaxCount => _maxCount;//최대 개수
    public int Count { get; set; }

    [SerializeField] private int _maxCount = 4;
    [SerializeField] private int _count = 1;
}
