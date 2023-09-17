using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public Image icon;//아이템의 이미지
    public Color selectedColor, notSelectedColor;//선택된 슬롯의 색깔 변경을 위한 변수
    private ItemInSlot originalItem;//Swap에 사용

    private void Awake()
    {
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

    public void OnDrop(PointerEventData eventData)//아이템을 슬롯에 놓았을 때
    {
        ItemInSlot item = eventData.pointerDrag.GetComponent<ItemInSlot>();
        if (transform.childCount == 0)//빈 슬롯일 때
        {
            item.parentAfterDrag = transform;
        }
        else//빈 슬롯이 아닐 경우 Swap
        {
            originalItem = transform.GetChild(0).GetComponent<ItemInSlot>();
            originalItem.parentAfterDrag = item.parentAfterDrag;
            originalItem.transform.SetParent(item.parentAfterDrag);

            item.parentAfterDrag = transform;
            item.transform.SetParent(transform);
        }
    }
}
