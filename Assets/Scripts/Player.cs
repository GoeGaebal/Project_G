using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviourPun
{
    [SerializeField] private GameObject inventoryUI;//가방 아이콘
    private GameObject inventoryManager;//인벤토리매니저
    private InventoryManager inventorymanager;//스크립트
    private InputAction quickSlotAction;
    
    private float moveSpeed = 1f;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    private void Awake()
    {
        quickSlotAction = new InputAction(binding: "<Mouse>/scroll/Y");
        quickSlotAction.performed += OnQuickSlot_Mouse;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inventoryManager = GameObject.Find("InventoryManager");
        inventorymanager = inventoryManager.GetComponent<InventoryManager>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnMove(InputValue value)
    {
        if (photonView.IsMine)
        {
            moveInput = value.Get<Vector2>();
        }
    }

    private void OnInventory()//가방 껐다 켜기
    {
        if (inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(false);
        }
        else
        {
            inventoryUI.SetActive(true);
        }
    }

    private void OnQuickSlot_Keyboard(InputValue value)
    {
        
    }

    private void OnQuickSlot_Mouse(InputAction.CallbackContext context)
    {
        float scrollValue = Mouse.current.scroll.ReadValue().normalized.y;

        if (scrollValue > 0)
        {
            inventorymanager.ChangeSelectedQuickSlot(inventorymanager.selectedSlot - 1);
        }
        else if (scrollValue < 0)
        {
            inventorymanager.ChangeSelectedQuickSlot(inventorymanager.selectedSlot + 1);
        }
    }


    private void OnEnable()
    {
        quickSlotAction.Enable();
    }

    private void OnDisable()
    {
        quickSlotAction.Disable();
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y,-10);
        }
    }
}
