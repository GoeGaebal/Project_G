using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager
{
    [HideInInspector] public Item[] inventorySlots;
    [HideInInspector] public int[] inventoryCount;
    [HideInInspector] public Item[] equipSlots;
    [HideInInspector] public int[] equipCount;
    [HideInInspector] public Item[] chestSlots;
    [HideInInspector] public int[] chestCount;
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
        inventoryCount = new int[_inventoryCount];
        equipCount = new int[_equipCount];
        chestCount = new int[_chestCount];
        inventorySlots[0] = Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/Sword");
        inventoryCount[0] = 1;
    }

    public void SpawnNewItem(Item item, UI_Slot slot)//슬롯에 새로운 아이템 추가
    {
        GameObject newItemGo = Managers.Resource.Instantiate("UI/SubItem/UI_Item", Vector3.zero, Quaternion.identity, parent: slot.transform);//테스트용
        UI_Item itemInSlot = newItemGo.GetComponent<UI_Item>();

        if (slot.isEquip)
        {
            itemInSlot.InitializeItem(item, equipCount[slot.invIndex]);
        }
        else if (slot.isChest)
        {
            itemInSlot.InitializeItem(item, chestCount[slot.invIndex]);
        }
        else
        {
            itemInSlot.InitializeItem(item, inventoryCount[slot.invIndex]);
        }
    }

    public void SpawnNewItemInEmptySlot(Item item, UI_Slot slot)
    {
        GameObject newItemGo = Managers.Resource.Instantiate("UI/SubItem/UI_Item", Vector3.zero, Quaternion.identity, parent: slot.transform);//테스트용
        UI_Item itemInSlot = newItemGo.GetComponent<UI_Item>();
        itemInSlot.InitializeItem(item, 1);
    }
}
