using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public UI_Inven ui_inven;
    public Item[] itemsToPickUp;

    public void PickUpItem(int id)
    {
        bool result = ui_inven.AddItem(itemsToPickUp[id]);
        if (result)
        {
            //Debug.Log("아이템 추가됨");
        }
        else
        {
            Debug.Log("아이템 추가 안 됨");
        }
    }
}
