using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //public int maxStackedItems = 4;
    public Slot[] slots;//전체 슬롯(퀵슬롯 포함)
    public Slot[] quickslots;//퀵슬롯만
    public GameObject IconPrefab;//InventoryItemPrefab

    public int selectedSlot = -1; //현재 선택된 퀵슬롯

    private void Start()
    {
        ChangeSelectedQuickSlot(0);//처음에는 1번째 퀵슬롯 선택되게
    }

    private void Update()
    {//키입력에 따른 퀵슬롯 선택 변화(추후 New Input System으로 변경)
        if (Managers.Input.PlayerActions.QuickSlot1.IsPressed())
        {
            ChangeSelectedQuickSlot(0);
        } else if (Managers.Input.PlayerActions.QuickSlot2.IsPressed())
        {
            ChangeSelectedQuickSlot(1);
        }
        else if (Managers.Input.PlayerActions.QuickSlot3.IsPressed())
        {
            ChangeSelectedQuickSlot(2);
        }
        else if (Managers.Input.PlayerActions.QuickSlot4.IsPressed())
        {
            ChangeSelectedQuickSlot(3);
        }
    }

    public void ChangeSelectedQuickSlot(int newValue)//선택된 슬롯 색깔 변경
    {
        if (newValue < 0)//퀵슬롯의 가장 왼쪽 칸에서 다시 왼쪽 칸으로 가면
        {
            newValue += quickslots.Length;//가장 오른쪽 칸으로
        }
        else if (newValue >= quickslots.Length)//가장 오른쪽 칸에서 다시 오른쪽으로 가면
        {
            newValue -= quickslots.Length;//가장 왼쪽으로
        }
        if (selectedSlot >= 0)
        {
            quickslots[selectedSlot].Deselect();//원래 선택되어 있던 슬롯 선택 해제
        }
        quickslots[newValue].Select();//새로운 슬롯 선택
        selectedSlot = newValue;//현재 선택 중인 슬롯 새로운 슬롯으로 변경


        //quick slot[selectedSlot].GetComponentInChildren<ItemInSlot>();으로 아이템 슬롯 가져온다
        //itemInSlot.item의 타입을 확인하여 칼인지 도끼인지 체크한 후 changeweapon
        // 이해랑이 바꿈
        ItemInSlot selectedItem = quickslots[selectedSlot].GetComponentInChildren<ItemInSlot>();
        if(selectedItem == null) return;

        Item slotItem = (selectedItem.item);
        if(slotItem != null && slotItem is EquipableItem)
        {
            Debug.Log(slotItem.GetType());
            ((EquipableItem)slotItem).ChangeEquipableItem();
        }
        
    }

    public ItemInSlot GetSelectedSlot()//현재 선택 중인 슬롯의 아이템 가져오기
    {
        return slots[selectedSlot].GetComponentInChildren<ItemInSlot>();
    }

    public bool AddItem(Item item)//아이템 추가
    {

        for(int i = 0; i < slots.Length; i++)//모든 슬롯을 돌면서
        {
            Slot slot = slots[i];
            ItemInSlot itemInSlot = slot.GetComponentInChildren<ItemInSlot>();
            
            if (itemInSlot != null &&//빈칸 아님
                itemInSlot.item == item &&//추가하려는 아이템과 동일한 아이템이 슬롯에 있음
                itemInSlot.item is CountableItem &&//이미 countable한 아이템이 인벤토리에 들어있을 때
                itemInSlot.count < ((CountableItem)itemInSlot.item).MaxCount)//최대 개수 미만
            {
                itemInSlot.count++;//개수 추가
                itemInSlot.RefreshCount();//텍스트 변경
                return true;//추가 완료함
            }
            
            else if (itemInSlot == null)//위의 모든 조건을 제외하고 빈칸을 만났을 때
            {
                SpawnNewItem(item, slot);//그냥 해당 슬롯에 아이템 추가
                return true;//생성 완료함
            }
        }

        return false;//인벤토리 빈 공간 없음
    }

    public void SpawnNewItem(Item item, Slot slot)//슬롯에 새로운 아이템 추가
    {
        GameObject newItemGo = Instantiate(IconPrefab, slot.transform);//테스트용
        ItemInSlot itemInSlot = newItemGo.GetComponent<ItemInSlot>();
        itemInSlot.InitializeItem(item);
    }
}
