using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemInSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _icon;
    [HideInInspector] public Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _icon.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _icon.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}
