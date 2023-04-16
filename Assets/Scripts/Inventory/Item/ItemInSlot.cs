using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemInSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _icon;//아이템의 이미지

    public Text countText;
    [HideInInspector] public Item item;//아이템
    [HideInInspector] public int count = 1;//아이템 개수
    [HideInInspector] public Transform parentAfterDrag;

    private void Start()
    {
        InitializeItem(item);
    }

    public void InitializeItem(Item newItem)//슬롯의 아이콘을 해당 아이템의 것으로 변경
    {
        item = newItem;
        _icon.sprite = newItem.Icon;
        RefreshCount();
    }

    public void RefreshCount()//아이템 개수 표시
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)//클릭했을 때
    {
        _icon.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)//드래그 중
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)//드랍했을 때
    {
        _icon.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}
