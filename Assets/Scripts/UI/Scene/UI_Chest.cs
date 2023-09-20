using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Chest : UI_Scene
{
    enum GameObjects
    {
        Chest,
        Contents,
    }

    enum Buttons
    {
        CloseButton,
    }

    private UI_Slot[] chestSlots;
    private GameObject _chest;
    private GameObject contents;
    public static System.Action open;

    private void Awake()
    {
        open = () => { OpenChest(); };
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        _chest = Get<GameObject>((int)GameObjects.Chest);
        contents = Get<GameObject>((int)GameObjects.Contents);

        chestSlots = new UI_Slot[Managers.Item.chestSlots.Length];
        for (int i = 0; i < Managers.Item.chestSlots.Length; i++)
        {
            chestSlots[i] = Managers.UI.MakeSubItem<UI_Slot>(parent: contents.transform);
            chestSlots[i].transform.localScale = Vector3.one;
            if(Managers.Item.chestSlots[i] != null)
            {
                Managers.Item.SpawnNewItem(Managers.Item.chestSlots[i], chestSlots[i]);
            }
        }

        _chest.SetActive(false);

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(CloseChest);
    }

    private void OnDestroy()
    {
        SaveItem();
    }

    public void SaveItem()
    {
        for(int i = 0; i < chestSlots.Length; i++)
        {//창고 저장
            if (chestSlots[i].transform.childCount >= 1)
            {
                Managers.Item.chestSlots[i] = chestSlots[i].transform.GetChild(0).GetComponent<UI_Item>().item;
            }
            else
            {
                Managers.Item.chestSlots[i] = null;
            }
        }
    }

    public void OpenChest()
    {
        _chest.SetActive(true);
        Managers.Input.PlayerActionMap.Enable();
    }

    public void CloseChest(PointerEventData evt)
    {
        _chest.SetActive(false);
        Managers.Input.PlayerActionMap.Disable();
    }
}