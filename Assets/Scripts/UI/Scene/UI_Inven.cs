using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Inven : UI_Scene//, IDataPersistence
{
    enum GameObjects
    {
        Inventory,
        Contents,
        Equips
    }

    enum Buttons
    {
        InventoryButton,
        CloseButton
    }

    enum EquipSlots
    {
        Weapon1,
        Hat,
        Upper,
        Lower,
        Shoes,
        Potion_1,
    }

    enum Images
    {
        WeaponImage,
        PotionImage
    }

    enum Texts
    {
        PotionAmountText
    }

    private GameObject _inventory;
    public UI_Slot[] slots;//전체 슬롯

    public UI_Slot[] equips;//장비창

    public static Image[] qSlots = new Image[2];
    public static Text potion1Text;
    private bool _canUsePotion = true;

    private bool _inventory_activeself;

    public static System.Func<Item, int> checkItem;

    private void Awake()
    {
        checkItem = CheckItem;
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<UI_Slot>(typeof(EquipSlots));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        _inventory = GetObject((int)GameObjects.Inventory);
        GameObject contents = GetObject((int)GameObjects.Contents);

        // 인벤토리 초기화
        foreach (Transform child in contents.transform)
        {
            if (child != null && child.gameObject.activeSelf)
                Managers.Resource.Destroy(child.gameObject);
        }

        slots = new UI_Slot[Managers.Item.inventorySlots.Length];

        // 실제 인벤토리 내부 contents 오브젝트 산하에 슬롯들 추가
        for (int i = 0; i < Managers.Item.inventorySlots.Length; i++)
        {
            slots[i] = Managers.UI.MakeSubItem<UI_Slot>(parent: contents.transform);
            slots[i].transform.localScale = Vector3.one;
            if(Managers.Item.inventorySlots[i] != null)
            {
                Managers.Item.SpawnNewItem(Managers.Item.inventorySlots[i], slots[i]);
            }
        }

        //장비창 슬롯 생성
        //0: 무기, 1: 모자, 2: 상의, 3: 하의, 4: 신발, 5: 포션
        equips = new UI_Slot[Enum.GetValues(typeof(EquipSlots)).Length];
        int idex = 0;
        foreach (EquipSlots slot in Enum.GetValues(typeof(EquipSlots)))
        {
            equips[idex] = Get<UI_Slot>((int)slot);
            equips[idex].isEquip = true;
            equips[idex].equipIndex = idex;
            if(Managers.Item.equipSlots[idex] != null)
            {
                Managers.Item.SpawnNewItem(Managers.Item.equipSlots[idex], equips[idex]);
            }
            idex++;
        }

        qSlots[0] = Get<Image>((int)Images.WeaponImage);
        qSlots[1] = Get<Image>((int)Images.PotionImage);
        potion1Text = Get<Text>((int)Texts.PotionAmountText);
        potion1Text.gameObject.SetActive(false);

        if (equips[0].transform.childCount >= 1)
        {
            ChangeQuickslotImage(0, equips[0].transform.GetChild(0).GetComponent<UI_Item>());
        }
        if (equips[5].transform.childCount >= 1)
        {
            ChangeQuickslotImage(1, equips[5].transform.GetChild(0).GetComponent<UI_Item>());
        }

        GetButton((int)Buttons.InventoryButton).onClick.AddListener(ClickInventoryButton);
        GetButton((int)Buttons.CloseButton).onClick.AddListener(ClickInventoryButton);
        Managers.Input.PlayerActions.Inventory.AddEvent(OnOffInventory);

        _inventory.SetActive(false);
        _inventory_activeself = _inventory.activeSelf;

        Managers.Network.ReceiveAddItemHandler -= AddItem;
        Managers.Network.ReceiveAddItemHandler += AddItem;

        // LoadData();
    }

    private void OnDestroy()
    {
        Managers.Input.PlayerActions.Inventory.RemoveEvent(OnOffInventory);
        Managers.Network.ReceiveAddItemHandler -= AddItem;
        SaveItem();
    }

    public void AddItem(int guid)
    {
        var item = Managers.Data.ItemDict[
            Managers.Object.LocalObjectsDict[guid].GetComponent<LootingItemController>().Item.ID];
        AddItem(item);
    }

    private void Update()
    {//키입력에 따른 퀵슬롯 선택 변화(추후 New Input System으로 변경)
        
        if (Managers.Input.PlayerActions.QuickSlot3.IsPressed())//포션 먹게
        {
            if (_canUsePotion)
            {
                _canUsePotion = false;
                StartCoroutine(PotionUse());
            }
        }
    }

    private IEnumerator PotionUse()
    {
        var potion = equips[5].transform.GetChild(0).GetComponent<UI_Item>();
        if (potion.count <= 1)
        {
            ((HealthPotionItem)potion.item).UsePotion(potion);
            qSlots[1].sprite = null;
            potion1Text.gameObject.SetActive(false);
            potion1Text.text = "0";
        }
        else
        {
            ((HealthPotionItem)potion.item).UsePotion(potion);
            potion1Text.text = potion.count.ToString();
        }
        yield return new WaitForSeconds(0.5f);
        _canUsePotion = true;
    }

    public bool AddItem(Item item)//아이템 추가
    {
        bool empty = false;
        int idx = 0;
        for (int i = 0; i < slots.Length; i++)//모든 슬롯을 돌면서
        {
            UI_Slot slot = slots[i];
            UI_Item itemInSlot = slot.GetComponentInChildren<UI_Item>();

            if (itemInSlot != null &&//빈칸 아님
                itemInSlot.item == item &&//추가하려는 아이템과 동일한 아이템이 슬롯에 있음
                itemInSlot.item is CountableItem &&//이미 countable한 아이템이 인벤토리에 들어있을 때
                itemInSlot.count < ((CountableItem)itemInSlot.item).MaxCount)//최대 개수 미만
            {
                itemInSlot.count++;//개수 추가
                itemInSlot.RefreshCount();//텍스트 변경
                return true;//추가 완료함
            }
            else if (itemInSlot == null && !empty)//위의 모든 조건을 제외하고 빈칸을 만났을 때
            {//가장 앞에 있는 빈칸 위치 저장해둠
                empty = true;
                idx = i;
            }
        }

        if (empty)
        {
            Managers.Item.SpawnNewItem(item, slots[idx]);//그냥 해당 슬롯에 아이템 추가
            return true;//생성 완료함
        }
        Debug.Log("인벤토리에 자리 없음");
        return false;//인벤토리 빈 공간 없음
    }
    /*
    public void SpawnNewItem(Item item, UI_Slot slot)//슬롯에 새로운 아이템 추가
    {
        GameObject newItemGo = Managers.Resource.Instantiate("UI/SubItem/UI_Item", Vector3.zero, Quaternion.identity, parent: slot.transform);//테스트용
        UI_Item itemInSlot = newItemGo.GetComponent<UI_Item>();
        itemInSlot.InitializeItem(item);
    }
    */
    public void SortItem()//아이템 정렬
    {
        bool swapped = true;
        while (swapped)
        {
            swapped = false;
            for (int i = 0; i < slots.Length - 1; i++)
            {
                UI_Slot a = slots[i];
                UI_Slot b = slots[i + 1];
                if (a.transform.childCount > 0 && b.transform.childCount > 0)
                {
                    int result1 = a.transform.GetChild(0).GetComponent<UI_Item>().item.ID
                        .CompareTo(b.transform.GetChild(0).GetComponent<UI_Item>().item.ID);
                    if (result1 > 0)
                    {
                        var tempTransform = a.transform;
                        a.transform.GetChild(0).SetParent(b.transform);
                        b.transform.GetChild(0).SetParent(tempTransform);
                        swapped = true;
                    }
                    else if (result1 == 0 && a.transform.GetChild(0).GetComponent<UI_Item>().item is CountableItem)
                    {
                        int result2 = a.transform.GetChild(0).GetComponent<UI_Item>().count
                        .CompareTo(b.transform.GetChild(0).GetComponent<UI_Item>().count);
                        if (result2 < 0)
                        {
                            var tempCount = a.transform.GetChild(0).GetComponent<UI_Item>().count;
                            a.transform.GetChild(0).GetComponent<UI_Item>().count = b.transform.GetChild(0).GetComponent<UI_Item>().count;
                            b.transform.GetChild(0).GetComponent<UI_Item>().count = tempCount;
                            a.transform.GetChild(0).GetComponent<UI_Item>().RefreshCount();
                            b.transform.GetChild(0).GetComponent<UI_Item>().RefreshCount();
                            swapped = true;
                        }
                    }
                }
                else if (a.transform.childCount == 0 && b.transform.childCount > 0)
                {
                    b.transform.GetChild(0).SetParent(a.transform);
                    swapped = true;
                }
            }
        }
    }

    public static void ChangeQuickslotImage(int index, UI_Item item)
    {
        qSlots[index].sprite = item.item.Icon;
        if (index == 1)//포션의 경우
        {
            potion1Text.gameObject.SetActive(true);
            potion1Text.text = item.count.ToString();
        }
    }

    public void ClickInventoryButton()//가방 버튼 클릭했을 때
    {
        if (_inventory_activeself)//인벤토리가 켜져 있으면 
        {
            _inventory.SetActive(false);//인벤토리 끔
        }
        else//인벤토리가 꺼져 있으면
        {
            _inventory.SetActive(true);//인벤토리 켬
        }

        _inventory_activeself = _inventory.activeSelf;
    }

    public void OnOffInventory(InputAction.CallbackContext evt)
    {
        if (_inventory_activeself)//인벤토리가 켜져 있으면 
        {
            _inventory.SetActive(false);//인벤토리 끔
        }
        else//인벤토리가 꺼져 있으면
        {
            _inventory.SetActive(true);//인벤토리 켬
        }

        _inventory_activeself = _inventory.activeSelf;
    }

    public int CheckItem(Item target)
    {
        int count = 0;

        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].transform.childCount < 1)
            {
                continue;
            }
            else
            {
                var slotItem = slots[i].transform.GetChild(0).GetComponent<UI_Item>();
                if (slotItem.item == target)
                {
                    count += slotItem.count;
                }
            }
        }

        return count;
    }

    public void SaveItem()
    {
        for(int i = 0; i < slots.Length; i++)
        {//인벤토리 저장
            if(slots[i].transform.childCount >= 1)
            {
                Managers.Item.inventorySlots[i] = slots[i].transform.GetChild(0).GetComponent<UI_Item>().item; 
            }
            else
            {
                Managers.Item.inventorySlots[i] = null;
            }
        }
        for(int i = 0; i < equips.Length; i++)
        {//장비창 저장
            if (equips[i].transform.childCount >= 1)
            {
                Managers.Item.equipSlots[i] = equips[i].transform.GetChild(0).GetComponent<UI_Item>().item;
            }
            else
            {
                Managers.Item.equipSlots[i] = null;
            }
        }
    }
    /*
    public void SaveData()
    {
        List<ulong> list = new();
        foreach (var slot in slots)
        {
            UI_Item item = null;
            if (slot.transform.childCount > 0)
                item = slot.transform.GetChild(0)?.GetComponent<UI_Item>();
            if (item != null)
                list.Add(Util.Vector2ulong(new Vector3(item.count, item.item.ID)));
            else
                list.Add(Util.Vector2ulong(new Vector3(0, -1)));
        }
        SaveData data = new();
        data.InventoryList = list;
        Managers.Data.Save(JsonUtility.ToJson(data), "Save.json");
    }

    public void LoadData()
    {
        SaveData loadDataList = JsonUtility.FromJson<SaveData>(Managers.Data.Load("Save.json"));
        if (loadDataList == null)
            return;
        for (int i = 0; i < loadDataList.InventoryList.Count; i++)
        {
            Vector2 info = Util.Ulong2Vector(loadDataList.InventoryList[i]);
            int count = (int)info.x;
            int id = (int)info.y;

            if (id > -1)
            {
                //TODO: GetComponentInChildren 너무 많이 쓰는 듯 + 사과만 생성된다.
                Item loadItem = Managers.Data.ItemDict[id];
                SpawnNewItem(loadItem, slots[i]);
                slots[i].GetComponentInChildren<UI_Item>().count = count;
                slots[i].GetComponentInChildren<UI_Item>().RefreshCount();
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
    */
}