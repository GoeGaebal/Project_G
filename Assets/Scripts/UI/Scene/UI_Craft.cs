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

    private bool _craftable;

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
            string t = Managers.Data.ItemDict[Managers.Data.CraftDict[i + 1].target].Name;
            Item temp = Managers.Data.ItemDict[Managers.Data.CraftDict[i + 1].target];
            //Item temp = Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{t}");
            _contents[i].SetSlot(i + 1, temp.Icon, Managers.Data.CraftDict[i + 1].targetAmount, temp.Name, temp.Tooltip);
        }
    }


    public void OpenCraftList()
    {
        _craftList.SetActive(true);
        _craft.SetActive(false);
        Managers.Input.PlayerActionMap.Disable();
    }

    public void CloseCraftList()
    {
        _craftList.SetActive(false);
        Managers.Input.PlayerActionMap.Enable();
    }

    public void OpenCraft(int id)
    {
        ClearMaterials();

        var recipe = Managers.Data.CraftDict[id];

        SelectedRecipeAmount[0] = recipe.targetAmount;
        SelectedRecipeAmount[1] = recipe.sourceAmount;
        SelectedRecipeAmount[2] = recipe.material1Amount;
        SelectedRecipeAmount[3] = recipe.material2Amount;

        if(SelectedRecipeAmount[0] > 0)
        {
            _target.gameObject.SetActive(true);
            SelectedRecipe[0] = Managers.Data.ItemDict[recipe.target];// Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{recipe.target}");
        }
        if (SelectedRecipeAmount[1] > 0)
        {
            _source.gameObject.SetActive(true);
            SelectedRecipe[1] = Managers.Data.ItemDict[recipe.source]; //Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{recipe.source}");
        }
        if (SelectedRecipeAmount[2] > 0)
        {
            _material_1.gameObject.SetActive(true);
            SelectedRecipe[2] = Managers.Data.ItemDict[recipe.material1]; //Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{recipe.material1}");
        }
        if (SelectedRecipeAmount[3] > 0)
        {
            _material_2.gameObject.SetActive(true);
            SelectedRecipe[3] = Managers.Data.ItemDict[recipe.material2]; //Managers.Resource.Load<Item>($"prefabs/UI/Inventory/Item/{recipe.material2}");
        }

        CheckRecipe();

        _craft.SetActive(true);
    }

    public void ClearMaterials()
    {
        _target.Clear();
        _source.Clear();
        _material_1.Clear();
        _material_2.Clear();
    }

    public void CheckRecipe()
    {
        int targetCount = 0, sourceCount = 0, material1Count = 0, material2Count = 0;

        targetCount = UI_Inven.checkItem(SelectedRecipe[0]);
        _target.SetSlot(SelectedRecipe[0].Icon, targetCount, SelectedRecipeAmount[0]);

        if (SelectedRecipeAmount[1] > 0)
        {
            sourceCount = UI_Inven.checkItem(SelectedRecipe[1]);
            _source.SetSlot(SelectedRecipe[1].Icon, sourceCount, SelectedRecipeAmount[1]);
        }

        if (SelectedRecipeAmount[2] > 0)
        {
            material1Count = UI_Inven.checkItem(SelectedRecipe[2]);
            _material_1.SetSlot(SelectedRecipe[2].Icon, material1Count, SelectedRecipeAmount[2]);
        }

        if (SelectedRecipeAmount[3] > 0)
        {
            material2Count = UI_Inven.checkItem(SelectedRecipe[3]);
            _material_2.SetSlot(SelectedRecipe[3].Icon, material2Count, SelectedRecipeAmount[3]);
        }

        if(sourceCount >= SelectedRecipeAmount[1] &&
            material1Count >= SelectedRecipeAmount[2] &&
            material2Count >= SelectedRecipeAmount[3])
        {
            _craftable = true;
        }
        else
        {
            _craftable = false;
        }
    }

    public void CloseCraft()
    {
        _craft.SetActive(false);
    }

    public void Craft()
    {
        if (_craftable)
        {
            for (int i = 1; i <= 3; i++)
            {
                if (SelectedRecipeAmount[i] > 0)
                {
                    UI_Inven.removeItems(SelectedRecipe[i], SelectedRecipeAmount[i]);
                }
            }
            for (int i = 0; i < SelectedRecipeAmount[0]; i++)
            {
                UI_Inven.additem(SelectedRecipe[0]);
            }
            CloseCraft();
        }
        else
        {
            Debug.Log("재료 개수 부족함");
        }
    }
}
