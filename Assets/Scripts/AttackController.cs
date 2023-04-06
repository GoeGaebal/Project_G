using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    
    public float attacCoefficient;
    
    private Player parentPlayerComponent;
    private IWeapon weaponController;
    private void Start() {
        parentPlayerComponent = GetComponentInParent<Player>();
        weaponController = new SwordAttackController();
    }

   

    private void OnTriggerEnter2D(Collider2D other) {

        Debug.Log("tirgger enter");
        if(other == null) return;
        
        BasicMonster damageable = other.GetComponent<BasicMonster>();
        if(damageable == null) return;

        Debug.Log("player attack");
        weaponController.Attack(other);
    }
    
}
