using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;

    private Vector2 moveInput;
    private Player targetPlayer = null;
    private void Awake() {
        Managers.Input.PlayerActions.Move.AddEvent(OnCameraMove);
    }

    private void Update() {
        if(Keyboard.current.f1Key.IsPressed())
            targetPlayer = null;
        else if(Keyboard.current.f2Key.IsPressed())
            targetPlayer = GetPlayer(0);
        else if(Keyboard.current.f3Key.IsPressed())
            targetPlayer = GetPlayer(1);
        else if(Keyboard.current.f4Key.IsPressed())
            targetPlayer = GetPlayer(2);
      

        if(targetPlayer == null)
        {
           
            transform.position += new Vector3(moveInput.x * moveSpeed * Time.deltaTime, moveInput.y * moveSpeed* Time.deltaTime, 0);
        }
        else
        {
            if(targetPlayer.GetComponent<Player>().IsDead) targetPlayer = null;
            else transform.position = targetPlayer.transform.position + new Vector3(0,0, - targetPlayer.transform.position.z + transform.position.z);
        }
        
    }
    public void OnCameraMove(InputAction.CallbackContext context)
    {
         moveInput = context.ReadValue<Vector2>();
    }   

    public void SetPosition(Vector3 initPosition)
    {
        transform.position = new Vector3(initPosition.x, initPosition.y, -10.0f);
    }

    private Player GetPlayer(int i)
    {
        List<int> playerIDs = new();
        foreach(int key in Managers.Object.PlayerDict.Keys)
        {
            playerIDs.Add(key);            
        }
        playerIDs.Sort();
        if(i < playerIDs.Count)
        {
            return Managers.Object.PlayerDict[playerIDs[i]];
        }
        else
        {
            return null;
        }
            
    }
}
