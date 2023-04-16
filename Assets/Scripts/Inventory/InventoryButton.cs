using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    public GameObject inventoryUI;
    public void OnClick()//가방 버튼 클릭했을 때
    {
        if (inventoryUI.activeSelf)//인벤토리가 켜져 있으면 
        {
            inventoryUI.SetActive(false);//인벤토리 끔
        }
        else//인벤토리가 꺼져 있으면
        {
            inventoryUI.SetActive(true);//인벤토리 켬
        }
    }
}
