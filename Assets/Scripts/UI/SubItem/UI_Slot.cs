using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : UI_Base
{
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

        item.parentBeforeDrag = transform;
    }
    
}
