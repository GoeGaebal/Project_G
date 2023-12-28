using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public UI_Inven ui_inven;
    private int[] itemsToPickUp = { 4001, 4101, 4102, 4103, 1001, 1002, 1003, 1004, 2001, 2002, 2003, 2004, 3001 };

    public void PickUpItem(int id)
    {
        ui_inven.AddItem(Managers.Data.ItemDict[itemsToPickUp[id]]);
    }

    public void SortItem()
    {
        ui_inven.SortItem();
    }
}
