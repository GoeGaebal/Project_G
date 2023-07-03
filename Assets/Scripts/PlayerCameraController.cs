using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;

    private Vector2 moveInput;


    private void Awake() {
        Managers.Input.PlayerActions.Move.AddEvent(OnCameraMove);
        Debug.Log("camera action added");
    }

    private void Update() {
        transform.position = new Vector3(transform.position.x + moveInput.x * moveSpeed * Time.deltaTime, transform.position.y + moveInput.y * moveSpeed* Time.deltaTime, transform.position.z);
    }
    public void OnCameraMove(InputAction.CallbackContext context)
    {
         moveInput = context.ReadValue<Vector2>();
    }   

    public void SetPosition(Vector3 initPosition)
    {
        transform.position = new Vector3(initPosition.x, initPosition.y, -10.0f);
    }
}
