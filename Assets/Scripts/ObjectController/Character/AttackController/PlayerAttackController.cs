using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum EnumWeaponList
{
    Sword, 
    //Axe
}
public class PlayerAttackController : AttackController
{
    [SerializeField] Sprite swordSprite;

    private EnumWeaponList currentWeapon;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        this.currentWeapon = EnumWeaponList.Sword;
        AddTargetLayer((int)EnumLayerMask.Monster);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void SyncWeapon(EnumWeaponList changeWeapon)
    {
        if(changeWeapon == currentWeapon) return;        
        switch(changeWeapon)
        {
            case EnumWeaponList.Sword:
                Debug.Log("sword change");
                spriteRenderer.sprite = swordSprite;
                currentWeapon = EnumWeaponList.Sword;
                break;
        }
        
    }
    public void ChangeWeapon(EnumWeaponList changeWeapon)
    {  
        if(changeWeapon == currentWeapon) return;

        switch(changeWeapon)
        {
            case EnumWeaponList.Sword:
                Debug.Log("sword change");
                spriteRenderer.sprite = swordSprite;
                currentWeapon = EnumWeaponList.Sword;
                break;
        }
       
    }

}

