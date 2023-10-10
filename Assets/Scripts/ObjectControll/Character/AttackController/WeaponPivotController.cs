using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class WeaponPivotController : NetworkObject
{
    [SerializeField]private float disFromBody; 
    private GameObject playerGameObject;
    // Start is called before the first frame update
    private Player playerComponent;
    void Start()
    {
        playerGameObject = transform.root.gameObject;
        playerComponent = playerGameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerComponent != Managers.Network.LocalPlayer) return;
        if (playerComponent.isDead) return;
          //Get the Screen positions of the object
         Vector3 positionOnScreen = playerGameObject.transform.position;
         
         //Get the Screen position of the mouse
         // Vector3 mouseOnScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         Vector3 mouseOnScreen = Camera.main.ScreenToWorldPoint(Managers.Input.UIActions.Point.ReadValue<Vector2>());
         //Get the angle between the points
         float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen) + 90.0f; //90도 보정
 
         //Rotate the object to face the mouse
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        transform.position = new Vector3(-disFromBody * Mathf.Sin(angle * Mathf.Deg2Rad), disFromBody* Mathf.Cos(angle* Mathf.Deg2Rad),0) + transform.root.position;

         float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
         return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }
    }
}
