using System;
using UnityEngine;
using UnityEngine.InputSystem;

public struct PlayerActions
{
    public InputAction Move;
    public InputAction Attack;
    public InputAction Inventory;
    public InputAction QuickslotKeyboard;
    public InputAction QuickslotMouse;
}

public class InputManager
{
    // Create an .inputactions asset.
    private const string PCBindingGroup = "PC";
    private InputActionAsset _asset;
    private InputControlScheme _pcControlScheme;
    private InputActionMap _playerActionMap;
    public PlayerActions PlayerActions;


    public void Init()
    {
        _asset = ScriptableObject.CreateInstance<InputActionAsset>();
        _pcControlScheme  = new InputControlScheme("PC", new InputControlScheme.DeviceRequirement[]
        {
            new(){ controlPath = "<Keyboard>" },
            new(){ controlPath = "<Mouse>" },
        },
            bindingGroup: PCBindingGroup);
        
        _asset.AddControlScheme(_pcControlScheme);
        _playerActionMap = _asset.AddActionMap("Player");

        // Actions
        PlayerActions.Move = _playerActionMap.AddAction("Move", groups: PCBindingGroup);
        PlayerActions.Attack = _playerActionMap.AddAction("Attack", type: InputActionType.Button, groups: PCBindingGroup);
        PlayerActions.Inventory = _playerActionMap.AddAction("Inventory", type: InputActionType.Button, groups: PCBindingGroup);
        PlayerActions.QuickslotKeyboard = _playerActionMap.AddAction("Quick-slot_keyboard", type: InputActionType.Button);
        PlayerActions.QuickslotMouse = _playerActionMap.AddAction("Quick-slot_Mouse", type: InputActionType.Value, groups: PCBindingGroup);

        
        
        // Binding
        PlayerActions.Move.AddCompositeBinding("2DVector(mode=1)", interactions: "Press")
            .With("Up", "<Gamepad>/leftStick/up",groups: PCBindingGroup)
            .With("Down", "<Gamepad>/leftStick/down",groups: PCBindingGroup)
            .With("Left", "<Gamepad>/leftStick/left", groups: PCBindingGroup)
            .With("Right", "<Gamepad>/leftStick/right", groups: PCBindingGroup);
        PlayerActions.Attack.AddBinding("<Mouse>/rightButton", groups: PCBindingGroup);
        PlayerActions.QuickslotKeyboard.AddBinding("<Keyboard>/1", groups: PCBindingGroup);
        PlayerActions.QuickslotKeyboard.AddBinding("<Keyboard>/2", groups: PCBindingGroup);
        PlayerActions.QuickslotKeyboard.AddBinding("<Keyboard>/3", groups: PCBindingGroup);
        PlayerActions.QuickslotKeyboard.AddBinding("<Keyboard>/4", groups: PCBindingGroup);
        PlayerActions.QuickslotMouse = _playerActionMap.AddAction("Quick-slot_Mouse", type: InputActionType.Value,"<Mouse>/scroll/y", groups: PCBindingGroup);
    }

    public void Binding(Action<InputAction.CallbackContext> action)
    {
        PlayerActions.Move.started += action;
    }
}
