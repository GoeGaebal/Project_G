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
    [SerializeField] private Transform pivot;

    private void Awake()
    {
        playerGameObject = transform.root.gameObject;
        playerComponent = playerGameObject.GetComponent<Player>();
    }

    void Update()
    {
        if(playerComponent != Managers.Network.LocalPlayer) return;
        if (playerComponent.isDead) return;

        //Get the Screen position of the mouse
        // Vector3 mouseOnScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseOnScreen = Camera.main.ScreenToWorldPoint(Managers.Input.UIActions.Point.ReadValue<Vector2>());
        
        Vector2 pos = pivot.position;
        Vector2 dir = (mouseOnScreen - pos).normalized;
        transform.position = dir * disFromBody + pos;
        transform.rotation = Quaternion.Euler(new Vector3(0.0f,0.0f,(Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg) + 90.0f));
    }
}
