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

    //private UI_Slot[] chestSlots;
    //private int _slotCount = 49;
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
        //TODO: UI_Item 드래그할 때 창고 UI 뒤로 숨는 버그 수정 필요
        Managers.UI.SetCanvas(gameObject, true);

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        _chest = Get<GameObject>((int)GameObjects.Chest);
        contents = Get<GameObject>((int)GameObjects.Contents);

        for(int i = 0; i < Managers.Item.chestSlots.Length; i++)
        {
            Managers.Item.chestSlots[i] = Managers.UI.MakeSubItem<UI_Slot>(parent: contents.transform);
            Managers.Item.chestSlots[i].transform.localScale = Vector3.one;
        }

        _chest.SetActive(false);
        
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(CloseChest);
    }

    public void OpenChest()
    {
        _chest.SetActive(true);
    }

    public void CloseChest(PointerEventData evt)
    {
        _chest.SetActive(false);
    }
}
