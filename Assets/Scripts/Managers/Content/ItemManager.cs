using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager
{
    [HideInInspector] public UI_Slot[] inventorySlots;
    [HideInInspector] public UI_Slot[] equipSlots;
    [HideInInspector] public UI_Slot[] chestSlots;
    [HideInInspector] public Image[] quickSlots;
    private int _inventoryCount = 24;
    private int _equipCount = 6;
    private int _chestCount = 49;
    private int _quickCount = 2;
    public void Init()
    {
        GameObject root = GameObject.Find("@Artifact");
        if (root == null)
        {
            root = new GameObject { name = "@Artifact" };
            UnityEngine.Object.DontDestroyOnLoad(root);
        }
        inventorySlots = new UI_Slot[_inventoryCount];
        equipSlots = new UI_Slot[_equipCount];
        chestSlots = new UI_Slot[_chestCount];
        quickSlots = new Image[_quickCount];
    }
}
