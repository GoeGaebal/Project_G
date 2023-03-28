using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)//아이템을 슬롯에 놓았을 때
    {
        if (transform.childCount == 0)
        {
            ItemInSlot item = eventData.pointerDrag.GetComponent<ItemInSlot>();
            item.parentAfterDrag = transform;
        }
    }
}
