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
        if(Input.GetKeyDown(KeyCode.F1))
            targetPlayer = null;
        else if(Input.GetKeyDown(KeyCode.F2))
            targetPlayer = GetPlayer(0);
        else if(Input.GetKeyDown(KeyCode.F3))
            targetPlayer = GetPlayer(1);
        else if(Input.GetKeyDown(KeyCode.F4))
            targetPlayer = GetPlayer(2);
      

        if(targetPlayer == null)
        {
           
            transform.position += new Vector3(moveInput.x * moveSpeed * Time.deltaTime, moveInput.y * moveSpeed* Time.deltaTime, 0);
        }
        else
        {
            if(targetPlayer.GetComponent<Player>().isDead) targetPlayer = null;
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
        foreach(int key in Managers.Network.PlayerDict.Keys)
        {
            playerIDs.Add(key);            
        }
        playerIDs.Sort();
        if(i < playerIDs.Count)
        {
            return Managers.Network.PlayerDict[playerIDs[i]];
        }
        else
        {
            return null;
        }
            
    }
}
