using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : CountableItem
{
    public void UseItem(ItemInSlot selectedSlot)//음식 먹기, 포션 섭취 등 아이템 사용
    {
        if (selectedSlot.item != null)
        {
            selectedSlot.count--;
            if (selectedSlot.count <= 0)//아이템 개수 0이면 아이템 삭제
            {
                selectedSlot.RevmoveItem();
            }
            else//아이템 개수 변경
            {
                selectedSlot.RefreshCount();
            }
        }
    }
}
