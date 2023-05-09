using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickUp;

    public void PickUpItem(int id)
    {
        bool result = inventoryManager.AddItem(itemsToPickUp[id]);
        if (result)
        {
            //Debug.Log("아이템 추가됨");
        }
        else
        {
            Debug.Log("아이템 추가 안 됨");
        }
    }

    public void GetSelectedItem()
    {
        ItemInSlot selectedSlot = inventoryManager.GetSelectedSlot();

        if (selectedSlot != null)
        {
            Debug.Log(selectedSlot.item + " 선택됨");
        }
        else
        {
            Debug.Log("아무것도 선택 안 됨");
        }
    }

    public void UseSelectedItem()
    {
        ItemInSlot selectedSlot = inventoryManager.GetSelectedSlot();
        Item currentItem = selectedSlot.item;

        if (selectedSlot.item is UsableItem)
        {
            ((UsableItem)selectedSlot.item).UseItem(selectedSlot);
            Debug.Log(selectedSlot.item + " 사용됨");
        }
        else
        {
            Debug.Log(selectedSlot.item + "사용 불가");
        }
    }
}
