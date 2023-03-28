using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int maxStackedItems = 4;
    public Slot[] slots;
    public GameObject IconPrefab;//InventoryItemPrefab

    int selectedSlot = -1;//현재 선택된 퀵슬롯

    private void Start()
    {
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeSelectedSlot(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSelectedSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeSelectedSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeSelectedSlot(3);
        }

    }

    void ChangeSelectedSlot(int newValue)//선택된 슬롯 색깔 변경
    {
        if (selectedSlot >= 0)
        {
            slots[selectedSlot].Deselect();
        }
        
        slots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(Item item)//아이템 추가
    {
        for(int i = 0; i < slots.Length; i++)
        {
            Slot slot = slots[i];
            ItemInSlot itemInSlot = slot.GetComponentInChildren<ItemInSlot>();
            
            if (itemInSlot != null &&
                itemInSlot.item == item &&
                itemInSlot.count < maxStackedItems &&
                itemInSlot.item.stackable)//이미 stackable한 아이템이 인벤토리에 최대 개수 이하로 들어있을 때
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
            else if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;//인벤토리 빈 공간 없음
    }

    void SpawnNewItem(Item item, Slot slot)
    {
        GameObject newItemGo = Instantiate(IconPrefab, slot.transform);//테스트용
        ItemInSlot itemInSlot = newItemGo.GetComponent<ItemInSlot>();
        itemInSlot.InitializeItem(item);
    }
}
