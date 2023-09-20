using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;




public enum EnumLayerMask
{
    Player = 6, Monster = 3, Mineral = 8
};



public class AttackController : MonoBehaviourPun
{
    
    protected Weapon weaponController; 


    private void OnTriggerEnter2D(Collider2D other) {

        if(other == null) return;
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable == null) return;
        if(weaponController.CheckAttackLayer((int)other.gameObject.layer))
        {
            damageable.OnDamage(10.0f);
        }

    }
    


}
