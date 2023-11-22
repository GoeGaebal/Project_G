using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : UI_Base
{
    public int invIndex = -1;

    public bool isChest = false;
    public bool isEquip = false;
    private int[] _itemIndex = { 10, 30, 31, 32, 33, 40 };
    private Item itemInThisSlot;//이 슬롯의 자식으로 있는 아이템
    private Player _player;
    public Item ItemInThisSlot
    {
        get { return itemInThisSlot; }
        set
        {
            if (itemInThisSlot != value)
            {//슬롯에 들어 있는 아이템이 바뀌면 아이템 매니저에 연동시켜줌
                itemInThisSlot = value;
                if (isEquip)//장비창의 경우에는 스탯 동기화도 같이
                {
                    Managers.Item.equipSlots[invIndex] = value;
                    if(invIndex < 5)
                    {
                        if(value != null)
                        {
                            EquipableItem equipItem = (EquipableItem)(value);
                            //_player.equipDamage[invIndex] = equipItem.AttackDamge;
                        }
                        else
                        {
                            //_player.equipDamage[invIndex] = 0;
                        }   
                    }
                }
                else if(isChest)
                {
                    Managers.Item.chestSlots[invIndex] = value;
                }
                else
                {
                    Managers.Item.inventorySlots[invIndex] = value;
                }
            }
        }
    }


    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        _player = Managers.Network.LocalPlayer;
        AddUIEvent(gameObject, OnDrop, Define.UIEvent.Drop);
        if(transform.childCount > 0)
        {
            ItemInThisSlot = transform.GetChild(0).GetComponent<UI_Item>().item;
        }
    }

    public void OnDrop(PointerEventData eventData) //아이템을 빈 슬롯에 놓았을 때
    {
        UI_Item currentItem = eventData.pointerDrag.GetComponent<UI_Item>();//현재 드래그하고 있는 아이템
        currentItem.parentSlot.itemInThisSlot = null;//원래 슬롯은 0으로
        if (isEquip)//슬롯이 장비창일 때
        {
            int checkIndex = currentItem.item.ID / 100;
            if (_itemIndex[invIndex] == checkIndex)
            {
                currentItem.parentSlot = currentItem.parentBeforeDrag.GetComponent<UI_Slot>();//아이템매니저 동기화
                currentItem.parentSlot.ItemInThisSlot = null;

                currentItem.parentBeforeDrag = transform;

                if (checkIndex == 10)//무기
                {//TODO: 무기 이미지 제대로 안 바뀜
                    UI_Inven.ChangeQuickslotImage(0, currentItem);
                    if (currentItem.item.ID == 1001)
                    {
                        PlayerAttackController.ChangeWeapon(EnumWeaponList.Sword);
                    }
                }
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
        else//슬롯이 인벤토리, 창고일 때
        {
            currentItem.parentSlot = currentItem.parentBeforeDrag.GetComponent<UI_Slot>();//아이템매니저 동기화
            currentItem.parentSlot.ItemInThisSlot = null;

            currentItem.parentBeforeDrag = transform;
        }

        ItemInThisSlot = currentItem.item;
    }
}
