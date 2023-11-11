using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;




public enum EnumLayerMask
{
    Player = 6, Monster = 3, Mineral = 8
};



public class AttackController : MonoBehaviour
{
    
    protected Weapon weaponController;

    protected virtual void OnTriggerEnter2D(Collider2D other) {

        if(other == null) return;
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable == null) return;
        if(weaponController.CheckAttackLayer((int)other.gameObject.layer))
        {
            damageable.OnDamage(20.0f);
        }

    }
}