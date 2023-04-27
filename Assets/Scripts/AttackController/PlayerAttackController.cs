using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackController : AttackController
{
    [SerializeField] Sprite swordSprite;
    [SerializeField] Sprite axeSprite;
    private Weapon meleeWeaponController;
    private Weapon pickaxWeaponController;

     private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meleeWeaponController = new MeleeWeaponController();
        pickaxWeaponController = new PickaxWeaponController();


        base.weaponController = meleeWeaponController;

        spriteRenderer = GetComponent<SpriteRenderer>();
        

        
    }


        public void ChangeWeapon(InputAction.CallbackContext context)
    {
       
        if(context.ReadValue<float>() > 0 && context.started)
        {
            Debug.Log("wheel up");
            spriteRenderer.sprite = axeSprite;
                
            
            
        }
        else if(context.ReadValue<float>() < 0 && context.started)
        {
            Debug.Log("wheel down");
            spriteRenderer.sprite = swordSprite;

                
        }
        spriteRenderer.sortingOrder = 3;
    }

}
