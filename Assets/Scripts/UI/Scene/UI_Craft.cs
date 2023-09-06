using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UI_Craft : UI_Scene
{
    enum GameObjects
    {
        CraftList,
        Craft,
        Content,
    }

    enum Buttons
    {
        CloseButton,
        CraftButton,
    }

    enum Slots
    {
        TargetItem,
        SourceItem,
        Material1,
        Material2,
    }

    private GameObject _craftList;
    private GameObject _craft;
    private GameObject _content;
    private UI_CraftMaterial _target;
    private UI_CraftMaterial _source;
    private UI_CraftMaterial _material_1;
    private UI_CraftMaterial _material_2;

    private UI_CraftSlot[] _contents;
    private int contentAmount = 1;

    public static System.Action open;

    private void Awake()
    {
        open = () => { OpenCraft(); };
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    public override void Init()
    {
        //base.init();
        Managers.UI.SetCanvas(gameObject, true);

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<UI_CraftSlot>(typeof(Slots));

        _craftList = Get<GameObject>((int)GameObjects.CraftList);
        _craft = Get<GameObject>((int)GameObjects.Craft);
        _content = Get<GameObject>((int)GameObjects.Content);

        for(int i = 0; i < contentAmount; i++)
        {
            _contents[i] = Managers.UI.MakeSubItem<UI_CraftSlot>(parent: _content.transform);
            string t = Managers.Data.CraftDict[i].target;
            Item temp = Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/Food/{t}");
            _contents[i].SetSlot(i, temp.Icon, Managers.Data.CraftDict[i].targetAmount, temp.name, temp.Tooltip);
        }

        _target = Get<UI_CraftMaterial>((int)Slots.TargetItem);
        _source = Get<UI_CraftMaterial>((int)Slots.SourceItem);
        _material_1 = Get<UI_CraftMaterial>((int)Slots.Material1);
        _material_2 = Get<UI_CraftMaterial>((int)Slots.Material2);

        Get<Button>((int)Buttons.CloseButton).gameObject.BindEvent(CloseCraftList);
        Get<Button>((int)Buttons.CraftButton).gameObject.BindEvent(Craft);
    }

    public void OpenCraftList()
    {
        _craftList.SetActive(true);
        _craft.SetActive(false);
    }

    public void CloseCraftList(PointerEventData evt)
    {
        _craftList.SetActive(false);
    }

    public void OpenCraft()
    {
        _craft.SetActive(true);
    }

    public void Craft(PointerEventData evt)
    {

    }
}
