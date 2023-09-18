using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Photon.Pun;

public enum EnumWeaponList
{
    Sword, 
    //Axe
}
public class PlayerAttackController : AttackController
{
    public static Action<EnumWeaponList> ChangeWeapon;
    [SerializeField] Sprite swordSprite;
    [SerializeField] Sprite axeSprite;
    private Weapon meleeWeaponController;
    //private Weapon pickaxWeaponController;
    private EnumWeaponList currentWeapon;
    private static GameObject localPlayer;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        ChangeWeapon += (EnumWeaponList changeWeapon) => 
        {
            _ChangeWeapon(changeWeapon);
        };
        meleeWeaponController = new MeleeWeaponController();
        //pickaxWeaponController = new PickaxWeaponController();


        base.weaponController = meleeWeaponController;
        this.currentWeapon = EnumWeaponList.Sword;

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if(photonView.IsMine)
        {
            localPlayer = this.gameObject;
        }        
    }

    [PunRPC]
    private void SyncWeapon(EnumWeaponList changeWeapon)
    {
        if(changeWeapon == currentWeapon) return;        
        switch(changeWeapon)
        {
            case EnumWeaponList.Sword:
                Debug.Log("sword change");
                spriteRenderer.sprite = swordSprite;
                weaponController = this.meleeWeaponController;
                currentWeapon = EnumWeaponList.Sword;
                break;
            //도끼가 없어져서 주석 처리
            // case EnumWeaponList.Axe:
            //     Debug.Log("axe change");
            //     spriteRenderer.sprite = axeSprite;
            //     weaponController = pickaxWeaponController;
            //     currentWeapon = EnumWeaponList.Axe;
            //     break;
            
        }
        spriteRenderer.sortingOrder = 3;
    }
    public void _ChangeWeapon(EnumWeaponList changeWeapon)
    {  
        if(changeWeapon == currentWeapon) return;
        if(!photonView.IsMine) return;
        
        switch(changeWeapon)
        {
            case EnumWeaponList.Sword:
                Debug.Log("sword change");
                spriteRenderer.sprite = swordSprite;
                weaponController = this.meleeWeaponController;
                currentWeapon = EnumWeaponList.Sword;
                break;
            //도끼가 없어져서 주석 처리
            // case EnumWeaponList.Axe:
            //     Debug.Log("axe change");
            //     spriteRenderer.sprite = axeSprite;
            //     weaponController = pickaxWeaponController;
            //     currentWeapon = EnumWeaponList.Axe;
            //     break;
            
        }
        spriteRenderer.sortingOrder = 3;
       
        photonView.RPC("SyncWeapon",RpcTarget.Others,currentWeapon);
    }

}


class MeleeWeaponController : Weapon
{
   public MeleeWeaponController()
   {
        AddTargetLayer((int)EnumLayerMask.Monster);
   }
}
//도끼가 없어져서 주석 처리
// class PickaxWeaponController : Weapon
// {
//    public PickaxWeaponController()
//    {
//         AddTargetLayer((int)EnumLayerMask.Mineral);
//    }
// }
