using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : UI_Base
{
    public Image icon;//아이템의 이미지
    public Color selectedColor, notSelectedColor;//선택된 슬롯의 색깔 변경을 위한 변수
    private UI_Item originalItem;//Swap에 사용

    private void Start()
    {
        Init();
    }
    
    public override void Init()
    {
        icon = GetComponent<Image>();
        AddUIEvent(gameObject,OnDrop,Define.UIEvent.Drop);
        Deselect();
    }

    public void Select()//선택된 슬롯 색깔 변경
    {
        icon.color = selectedColor;
    }

    public void Deselect()//선택되지 않게 된 슬롯 색깔 변경
    {
        icon.color = notSelectedColor;
    }
    
    public void OnDrop(PointerEventData eventData) //아이템을 슬롯에 놓았을 때
    {
        UI_Item item = eventData.pointerDrag.GetComponent<UI_Item>();
        if (transform.childCount == 0) //빈 슬롯일 때
        {
            item.parentAfterDrag = transform;
        }
        else //빈 슬롯이 아닐 경우 Swap
        {
            originalItem = transform.GetChild(0).GetComponent<UI_Item>();
            originalItem.parentAfterDrag = item.parentAfterDrag;
            originalItem.transform.SetParent(item.parentAfterDrag);

            item.parentAfterDrag = transform;
            item.transform.SetParent(transform);
        }
    }
}
