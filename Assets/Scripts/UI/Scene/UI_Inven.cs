using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        //QuickSlot,
        Inventory,
        Contents,
    }

    enum Buttons
    {
        InventoryButton,
        CloseButton
    }

    private GameObject _inventory;
    public UI_Slot[] slots;//전체 슬롯(퀵슬롯 포함)
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

        _inventory = GetObject((int)GameObjects.Inventory);
        GameObject contents = GetObject((int)GameObjects.Contents);

        // 인벤토리 초기화
        foreach (Transform child in contents.transform)
        {
            if (child != null && child.gameObject.activeSelf)
                Managers.Resource.Destroy(child.gameObject);
        }

        slots = new UI_Slot[InventorySlotCount];

        // 실제 인벤토리 내부 contents 오브젝트 산하에 슬롯들 추가
        for (int i = 0; i < InventorySlotCount; i++)
        {
            slots[i] = Managers.UI.MakeSubItem<UI_Slot>(parent : contents.transform);
            slots[i].transform.localScale = Vector3.one;
        }

        GetButton((int)Buttons.InventoryButton).gameObject.BindEvent(ClickInventoryButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClickInventoryButton);
        Managers.Input.PlayerActions.Inventory.AddEvent(OnOffInventory);
        
        _inventory.SetActive(false);
        _inventory_activeself = _inventory.activeSelf;

        Managers.Network.ReceiveAddItemHandler += AddItem;
    }

    public void AddItem(int guid)
    {
        AddItem(Managers.Object.LocalObjectsDict[guid].GetComponent<LootingItemController>().GetItem);
    }
    
    private void Update()
    {//키입력에 따른 퀵슬롯 선택 변화(추후 New Input System으로 변경)
        if (Managers.Input.PlayerActions.QuickSlot1.IsPressed())//무기 들게
        {

        } else if (Managers.Input.PlayerActions.QuickSlot2.IsPressed())//도구 들게
        {

        }
        else if (Managers.Input.PlayerActions.QuickSlot3.IsPressed())//포션 먹게
        {

        }
        else if (Managers.Input.PlayerActions.QuickSlot4.IsPressed())//포션 2 먹게
        {

        }
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

    public void SortItem()//아이템 정렬
    {

    }
    
    public void ClickInventoryButton(PointerEventData evt)//가방 버튼 클릭했을 때
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
}
