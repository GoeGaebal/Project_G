using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager
{
    [HideInInspector] public Item[] inventorySlots;
    [HideInInspector] public Item[] equipSlots;
    [HideInInspector] public Item[] chestSlots;
    private int _inventoryCount = 24;
    private int _equipCount = 6;
    private int _chestCount = 49;
    public void Init()
    {
        GameObject root = GameObject.Find("@Item");
        if (root == null)
        {
            root = new GameObject { name = "@Item" };
            UnityEngine.Object.DontDestroyOnLoad(root);
        }
        inventorySlots = new Item[_inventoryCount];
        equipSlots = new Item[_equipCount];
        chestSlots = new Item[_chestCount];
        inventorySlots[0] = Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/Sword");
    }

    public void SpawnNewItem(Item item, UI_Slot slot)//슬롯에 새로운 아이템 추가
    {
        GameObject newItemGo = Managers.Resource.Instantiate("UI/SubItem/UI_Item", Vector3.zero, Quaternion.identity, parent: slot.transform);//테스트용
        UI_Item itemInSlot = newItemGo.GetComponent<UI_Item>();
        itemInSlot.InitializeItem(item);
    }
}
