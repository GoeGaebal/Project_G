using System;
using UnityEngine;
using UnityEngine.InputSystem;

public struct PlayerActions
{
    public InputAction Move;
    public InputAction Attack;
    public InputAction Inventory;
    public InputAction QuickSlot1;
    public InputAction QuickSlot2;
    public InputAction QuickSlot3;
    public InputAction QuickSlot4;
    public InputAction ScrollQuickSlot;
}

public struct UIActions
{
    public InputAction Navigate;
    public InputAction Point;
    public InputAction Submit;
    public InputAction Cancel;
    public InputAction Click;
    public InputAction MiddleClick;
    public InputAction RightClick;
    public InputAction ScrollWheel;
}


public class InputManager
{
    // Create an .inputactions asset.
    private const string PCBindingGroup = "PC";
    private InputActionAsset _asset;
    private InputControlScheme _pcControlScheme;
    private InputActionMap _playerActionMap;
    private InputActionMap _uiActionMap;
    public PlayerActions PlayerActions;
    public UIActions UIActions;
    // public DefaultInputActions.UIActions UIActions;
    
    


    public void Init()
    {
        _asset = ScriptableObject.CreateInstance<InputActionAsset>();
        _pcControlScheme  = new InputControlScheme("PC", new InputControlScheme.DeviceRequirement[]
        {
            new(){ controlPath = "<Keyboard>" },
            new(){ controlPath = "<Mouse>" },
        });

        _asset.AddControlScheme(_pcControlScheme);
        _playerActionMap = _asset.AddActionMap("Player");
        _uiActionMap = _asset.AddActionMap("UI");

        // Actions
        PlayerActions.Move = _playerActionMap.AddAction("Move", type: InputActionType.Value);
        PlayerActions.Attack = _playerActionMap.AddAction("Attack", type: InputActionType.Button);
        PlayerActions.Inventory = _playerActionMap.AddAction("Inventory", type: InputActionType.Button);
        PlayerActions.QuickSlot1 = _playerActionMap.AddAction("QuickSlot1", type: InputActionType.Button);
        PlayerActions.QuickSlot2 = _playerActionMap.AddAction("QuickSlot2", type: InputActionType.Button);
        PlayerActions.QuickSlot3 = _playerActionMap.AddAction("QuickSlot3", type: InputActionType.Button);
        PlayerActions.QuickSlot4 = _playerActionMap.AddAction("QuickSlot4", type: InputActionType.Button);
        PlayerActions.ScrollQuickSlot = _playerActionMap.AddAction("ScrollQuickSlot", type: InputActionType.Value);

        UIActions.Navigate = _uiActionMap.AddAction("Navigate", type: InputActionType.PassThrough);
        UIActions.Submit = _uiActionMap.AddAction("Submit", type: InputActionType.Button);
        UIActions.Cancel = _uiActionMap.AddAction("Cancel", type: InputActionType.Button);
        UIActions.Point = _uiActionMap.AddAction("Point", type: InputActionType.PassThrough);
        UIActions.ScrollWheel = _uiActionMap.AddAction("ScrollWheel", type: InputActionType.PassThrough);
        UIActions.Click = _uiActionMap.AddAction("Click", type: InputActionType.Button);
        UIActions.MiddleClick = _uiActionMap.AddAction("MiddleClick", type: InputActionType.Button);
        UIActions.RightClick = _uiActionMap.AddAction("RightClick", type: InputActionType.Button);

        // Binding
        PlayerActions.Move.AddCompositeBinding("2DVector(mode=1)")
            .With("Up", "<Keyboard>/upArrow",groups: PCBindingGroup)
            .With("Up", "<Keyboard>/w",groups: PCBindingGroup)
            .With("Down", "<Keyboard>/downArrow",groups: PCBindingGroup)
            .With("Down", "<Keyboard>/s",groups: PCBindingGroup)
            .With("Left", "<Keyboard>/leftArrow", groups: PCBindingGroup)
            .With("Left", "<Keyboard>/a", groups: PCBindingGroup)
            .With("Right", "<Keyboard>/rightArrow", groups: PCBindingGroup)
            .With("Right", "<Keyboard>/d", groups: PCBindingGroup);
        PlayerActions.Attack.AddBinding("<Mouse>/rightButton", groups: PCBindingGroup);
        PlayerActions.Inventory.AddBinding("<Keyboard>/i", groups: PCBindingGroup);
        PlayerActions.QuickSlot1.AddBinding("<Keyboard>/1", groups: PCBindingGroup);
        PlayerActions.QuickSlot2.AddBinding("<Keyboard>/2", groups: PCBindingGroup);
        PlayerActions.QuickSlot3.AddBinding("<Keyboard>/3", groups: PCBindingGroup);
        PlayerActions.QuickSlot4.AddBinding("<Keyboard>/4", groups: PCBindingGroup);
        PlayerActions.ScrollQuickSlot.AddBinding("<Mouse>/scroll/y", groups: PCBindingGroup);
        
        UIActions.Navigate.AddCompositeBinding("2DVector(mode=1)")
            .With("Up", "<Keyboard>/w",groups: PCBindingGroup)
            .With("Down", "<Keyboard>/s",groups: PCBindingGroup)
            .With("Left", "<Keyboard>/a", groups: PCBindingGroup)
            .With("Right", "<Keyboard>/d", groups: PCBindingGroup);
        UIActions.Submit.AddBinding("*/{Submit}", groups: PCBindingGroup);
        UIActions.Cancel.AddBinding("*/{Cancel}", groups: PCBindingGroup);
        UIActions.Point.AddBinding("<Mouse>/position", groups: PCBindingGroup);
        UIActions.ScrollWheel.AddBinding("<Mouse>/scroll/y", groups: PCBindingGroup);
        UIActions.MiddleClick.AddBinding("<Mouse>/leftButton", groups: PCBindingGroup);
        UIActions.MiddleClick.AddBinding("<Mouse>/middleButton", groups: PCBindingGroup);
        UIActions.RightClick.AddBinding("<Mouse>/rightButton", groups: PCBindingGroup);

        // Initial Check
        UIActions.Click.wantsInitialStateCheck = true;

        _asset.Enable();
    }
}
