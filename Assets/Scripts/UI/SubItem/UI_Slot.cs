using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : UI_Base
{
    public bool isEquip = false;
    public int equipIndex = -1;
    private int[] _itemIndex = { 10, 30, 31, 32, 33, 40 };

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
        UI_Item item = eventData.pointerDrag.GetComponent<UI_Item>();//현재 드래그하고 있는 아이템
        if (isEquip)//슬롯이 장비창일 때
        {
            int checkIndex = item.item.ID / 100;
            if (_itemIndex[equipIndex] == checkIndex)
            {
                item.parentBeforeDrag = transform;
                if (checkIndex == 10)//무기
                {
                    UI_Inven.ChangeQuickImage(0, item);
                    if (GetComponent<UI_Item>().item.ID == 1001)
                    {
                        PlayerAttackController.ChangeWeapon(EnumWeaponList.Sword);
                    }
                    else if(GetComponent<UI_Item>().item.ID == 1002)
                    {
                        PlayerAttackController.ChangeWeapon(EnumWeaponList.Axe);
                    }
                }
                /*else if (checkIndex == 20)
                {
                    UI_Inven.ChangeQuickImage(1, item);
                }*/
                else if (checkIndex == 40)//포션
                {
                    UI_Inven.ChangeQuickImage(1, item);
                }
                else
                {
                    return;
                }
            }
        }
        else//슬롯이 인벤토리일 때
        {
            item.parentBeforeDrag = transform;
        }
    }
}
