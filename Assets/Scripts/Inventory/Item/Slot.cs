using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public Image icon;
    public Color selectedColor, notSelectedColor;
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
        if (transform.childCount == 0)//빈칸으로 이동
        {
            item.parentAfterDrag = transform;
        }
        else//swap
        {
            originalItem = transform.GetChild(0).GetComponent<ItemInSlot>();
            originalItem.parentAfterDrag = item.parentAfterDrag;
            originalItem.transform.SetParent(item.parentAfterDrag);

            item.parentAfterDrag = transform;
            item.transform.SetParent(transform);
        }
    }
}
