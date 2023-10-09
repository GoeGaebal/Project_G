using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : UI_Base
{
    public int invIndex = -1;

    public bool isChest = false;
    public bool isEquip = false;
    private int[] _itemIndex = { 10, 30, 31, 32, 33, 40 };
    private UI_Item itemInThisSlot;
    public UI_Item ItemInThisSlot
    {
        get { return itemInThisSlot; }
        set
        {
            if (itemInThisSlot != value)
            {//스탯 동기화를 위해 슬롯에 들어 있는 아이템이 바뀌면 아이템 매니저에 연동시켜줌
                itemInThisSlot = value;
                if (isEquip)
                {
                    //Managers.Item.equipSlots[invIndex] = value.item;
                }
                else
                {
                    //Managers.Item.inventorySlots[invIndex] = value.item;
                }
                Debug.Log("MyVariable changed to: " + itemInThisSlot);
            }
        }
    }


    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        AddUIEvent(gameObject, OnDrop, Define.UIEvent.Drop);
    }

    public void OnDrop(PointerEventData eventData) //아이템을 빈 슬롯에 놓았을 때
    {
        UI_Item currentItem = eventData.pointerDrag.GetComponent<UI_Item>();//현재 드래그하고 있는 아이템
        if (isEquip)//슬롯이 장비창일 때
        {
            int checkIndex = currentItem.item.ID / 100;
            if (_itemIndex[invIndex] == checkIndex)
            {
                currentItem.parentBeforeDrag = transform;
                if (checkIndex == 10)//무기
                {//TODO: 무기 이미지 제대로 안 바뀜
                    UI_Inven.ChangeQuickslotImage(0, currentItem);
                    if (currentItem.item.ID == 1001)
                    {
                        PlayerAttackController.ChangeWeapon(EnumWeaponList.Sword);
                    }
                }
                /*else if (checkIndex == 20)
                {
                    UI_Inven.ChangeQuickImage(1, item);
                }*/
                else if (checkIndex == 40)//포션
                {
                    UI_Inven.ChangeQuickslotImage(1, currentItem);
                }
                else
                {
                    return;
                }
            }
        }
        else//슬롯이 인벤토리일 때
        {
            currentItem.parentBeforeDrag = transform;
        }

        ItemInThisSlot = currentItem;
    }
}
