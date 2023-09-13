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

    //0: target,  1: source,  2: material1,  3: material2
    private Item[] SelectedRecipe = new Item[4];
    private int[] SelectedRecipeAmount = new int[4];

    public static System.Action<int> open;
    public static System.Action openUI;

    private void Awake()
    {
        open = (n) => { OpenCraft(n); };
        openUI = () => { OpenCraftList(); };
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
        Bind<UI_CraftMaterial>(typeof(Slots));

        _craftList = Get<GameObject>((int)GameObjects.CraftList);
        _craft = Get<GameObject>((int)GameObjects.Craft);
        _craft.SetActive(false);
        _craftList.SetActive(false);
        _content = Get<GameObject>((int)GameObjects.Content);
        _target = Get<UI_CraftMaterial>((int)Slots.TargetItem);
        _source = Get<UI_CraftMaterial>((int)Slots.SourceItem);
        _material_1 = Get<UI_CraftMaterial>((int)Slots.Material1);
        _material_2 = Get<UI_CraftMaterial>((int)Slots.Material2);

        Get<Button>((int)Buttons.CloseButton).onClick.AddListener(CloseCraftList);
        Get<Button>((int)Buttons.CraftButton).onClick.AddListener(Craft);

        _contents = new UI_CraftSlot[Managers.Data.CraftDict.Count];
        for(int i = 0; i < _contents.Length; i++)
        {
            _contents[i] = Managers.UI.MakeSubItem<UI_CraftSlot>(parent: _content.transform);
            _contents[i].Init();
            string t = Managers.Data.CraftDict[i + 1].target;
            Item temp = Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{t}");
            _contents[i].SetSlot(i + 1, temp.Icon, Managers.Data.CraftDict[i + 1].targetAmount, temp.Name, temp.Tooltip);
        }
    }

    public void OpenCraftList()
    {
        _craftList.SetActive(true);
        _craft.SetActive(false);
    }

    public void CloseCraftList()
    {
        _craftList.SetActive(false);
    }

    public void OpenCraft(int id)
    {
        var recipe = Managers.Data.CraftDict[id];

        SelectedRecipe[0] = Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{recipe.target}");
        SelectedRecipe[1] = Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{recipe.source}");
        SelectedRecipe[2] = Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{recipe.material1}");
        SelectedRecipe[3] = Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{recipe.material2}");
        SelectedRecipeAmount[0] = recipe.targetAmount;
        SelectedRecipeAmount[1] = recipe.sourceAmount;
        SelectedRecipeAmount[2] = recipe.material1Amount;
        SelectedRecipeAmount[3] = recipe.material2Amount;

        _target.SetSlot(SelectedRecipe[0].Icon, SelectedRecipeAmount[0]);
        _source.SetSlot(SelectedRecipe[1].Icon, SelectedRecipeAmount[1]);
        _material_1.SetSlot(SelectedRecipe[2].Icon, SelectedRecipeAmount[2]);
        _material_2.SetSlot(SelectedRecipe[3].Icon, SelectedRecipeAmount[3]);

        _craft.SetActive(true);
    }

    public void Craft()
    {

    }
}
