using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Inven : UI_Scene, IDataPersistence
{
    enum GameObjects
    {
        QuickSlot,
        Inventory,
        Contents,
    }

    enum Buttons
    {
        InventoryButton,
        CloseButton
    }

    private GameObject _inventory;
    private GameObject _quickSlotsPanel;
    public UI_Slot[] slots;//전체 슬롯(퀵슬롯 포함)
    public UI_Slot[] quickslots;//퀵슬롯만
    // public GameObject IconPrefab;//InventoryItemPrefab

    public int selectedSlot = -1; //현재 선택된 퀵슬롯
    public int QuickSlotCount = 4;
    public int InventorySlotCount = 16;

    private bool _inventory_activeself;
    

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        _quickSlotsPanel = GetObject((int)GameObjects.QuickSlot);
        _inventory = GetObject((int)GameObjects.Inventory);
        GameObject contents = GetObject((int)GameObjects.Contents);
        
        // 퀵슬롯 초기화
        foreach (Transform child in _quickSlotsPanel.transform)
        {
            if (child != null && child.gameObject.activeSelf)
                Managers.Resource.Destroy(child.gameObject);
        }
        // 인벤토리 초기화
        foreach (Transform child in contents.transform)
        {
            if (child != null && child.gameObject.activeSelf)
                Managers.Resource.Destroy(child.gameObject);
        }

        slots = new UI_Slot[QuickSlotCount+InventorySlotCount];
        quickslots = new UI_Slot[QuickSlotCount];
        
        // 퀵슬롯에 슬롯 추가
        for (int i = 0; i < QuickSlotCount; i++)
        {
            quickslots[i] = Managers.UI.MakeSubItem<UI_Slot>(parent : _quickSlotsPanel.transform);
            slots[i] = quickslots[i];
            slots[i].transform.localScale = Vector3.one;
        }

        // 실제 인벤토리 내부 contents 오브젝트 산하에 슬롯들 추가
        for (int i = QuickSlotCount; i < QuickSlotCount+InventorySlotCount; i++)
        {
            slots[i] = Managers.UI.MakeSubItem<UI_Slot>(parent : contents.transform);
            slots[i].transform.localScale = Vector3.one;
        }

        GetButton((int)Buttons.InventoryButton).onClick.AddListener(ClickInventoryButton);
        GetButton((int)Buttons.CloseButton).onClick.AddListener(ClickInventoryButton);
        Managers.Input.PlayerActions.ScrollQuickSlot.AddEvent(OnQuickSlot_Mouse);
        Managers.Input.PlayerActions.Inventory.AddEvent(OnOffInventory);
        
        _inventory.SetActive(false);
        _inventory_activeself = _inventory.activeSelf;

        Managers.Network.ReceiveAddItemHandler -= AddItem;
        Managers.Network.ReceiveAddItemHandler += AddItem;

        LoadData();
    }

    private void OnDestroy()
    {
        Managers.Input.PlayerActions.ScrollQuickSlot.RemoveEvent(OnQuickSlot_Mouse);
        Managers.Input.PlayerActions.Inventory.RemoveEvent(OnOffInventory);
        Managers.Network.ReceiveAddItemHandler -= AddItem;
    }

    public void AddItem(int guid)
    {
        var item = Managers.Data.ItemDict[
            Managers.Object.LocalObjectsDict[guid].GetComponent<LootingItemController>().Item.ID];
        AddItem(item);
    }
    
    private void Update()
    {//키입력에 따른 퀵슬롯 선택 변화(추후 New Input System으로 변경)
        if (Managers.Input.PlayerActions.QuickSlot1.IsPressed())
        {
            ChangeSelectedQuickSlot(0);
        } else if (Managers.Input.PlayerActions.QuickSlot2.IsPressed())
        {
            ChangeSelectedQuickSlot(1);
        }
        else if (Managers.Input.PlayerActions.QuickSlot3.IsPressed())
        {
            ChangeSelectedQuickSlot(2);
        }
        else if (Managers.Input.PlayerActions.QuickSlot4.IsPressed())
        {
            ChangeSelectedQuickSlot(3);
        }
    }
    
    public void ChangeSelectedQuickSlot(int newValue)//선택된 슬롯 색깔 변경
    {
        if (newValue < 0)//퀵슬롯의 가장 왼쪽 칸에서 다시 왼쪽 칸으로 가면
        {
            newValue += quickslots.Length;//가장 오른쪽 칸으로
        }
        else if (newValue >= quickslots.Length)//가장 오른쪽 칸에서 다시 오른쪽으로 가면
        {
            newValue -= quickslots.Length;//가장 왼쪽으로
        }
        if (selectedSlot >= 0)
        {
            quickslots[selectedSlot].Deselect();//원래 선택되어 있던 슬롯 선택 해제
        }
        quickslots[newValue].Select();//새로운 슬롯 선택
        selectedSlot = newValue;//현재 선택 중인 슬롯 새로운 슬롯으로 변경
        
        //quick slot[selectedSlot].GetComponentInChildren<ItemInSlot>();으로 아이템 슬롯 가져온다
        //itemInSlot.item의 타입을 확인하여 칼인지 도끼인지 체크한 후 changeweapon
        // 이해랑이 바꿈
        UI_Item selectedItem = quickslots[selectedSlot].GetComponentInChildren<UI_Item>();
        if(selectedItem == null) return;

        Item slotItem = (selectedItem.item);
        if(slotItem != null && slotItem is EquipableItem)
        {
            Debug.Log(slotItem.GetType());
            ((EquipableItem)slotItem).ChangeEquipableItem();
        }
    }
    
    public UI_Item GetSelectedSlot()//현재 선택 중인 슬롯의 아이템 가져오기
    {
        return slots[selectedSlot].GetComponentInChildren<UI_Item>();
    }
    
    public bool AddItem(Item item)//아이템 추가
    {
        for(int i = 0; i < slots.Length; i++)//모든 슬롯을 돌면서
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
            else if (itemInSlot == null)//위의 모든 조건을 제외하고 빈칸을 만났을 때
            {
                SpawnNewItem(item, slot);//그냥 해당 슬롯에 아이템 추가
                return true;//생성 완료함
            }
        }

        return false;//인벤토리 빈 공간 없음
    }

    public void SpawnNewItem(Item item, UI_Slot slot)//슬롯에 새로운 아이템 추가
    {
        GameObject newItemGo = Managers.Resource.Instantiate("UI/SubItem/UI_Item",Vector3.zero, Quaternion.identity, parent: slot.transform);//테스트용
        UI_Item itemInSlot = newItemGo.GetComponent<UI_Item>();
        itemInSlot.InitializeItem(item);
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
    
    public void OnQuickSlot_Mouse(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 scrollValue = context.ReadValue<Vector2>();

            if (scrollValue.y > 0)
            {
                ChangeSelectedQuickSlot(selectedSlot - 1);
            }
            else if (scrollValue.y < 0)
            {
                ChangeSelectedQuickSlot(selectedSlot + 1);
            }
        }
    }

    public void OnOffInventory(InputAction.CallbackContext context)
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
}
