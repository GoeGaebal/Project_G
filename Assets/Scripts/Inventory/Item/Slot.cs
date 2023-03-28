using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public Image icon;
    public Color selectedColor, notSelectedColor;

    private void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        icon.color = selectedColor;
    }

    public void Deselect()
    {
        icon.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)//아이템을 슬롯에 놓았을 때
    {
        if (transform.childCount == 0)
        {
            ItemInSlot item = eventData.pointerDrag.GetComponent<ItemInSlot>();
            item.parentAfterDrag = transform;
        }
    }
}
