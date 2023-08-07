using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : UI_Base
{
    public bool isEquip = false;
    public int equipIndex = -1;
    public int[] itemIndex = { 10, 20, 30, 31, 32, 33, 34, 40 };

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
            //if(item.item is EquipableItem || item.item is UsableItem)
            //{
                if(itemIndex[equipIndex] == item.item.ID / 100)
                {
                    item.parentBeforeDrag = transform;
                }
            //}
            else
            {
                return;
            }
        }
        else//슬롯이 인벤토리일 때
        {
            item.parentBeforeDrag = transform;
        }
    }
}
