using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    
    public float attacCoefficient;
    
    private Player parentPlayerComponent;
    private IWeapon weaponController;
    [SerializeField] private LayerMask targetLayerMask;
    private void Start() {
        weaponController = new MeleeAttackController();
    }

   

    private void OnTriggerEnter2D(Collider2D other) {

        Debug.Log("tirgger enter");
        if(other == null) return;
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable == null) return;
        
        if(IsInLayerMask(other.gameObject))
        {
            weaponController.Attack(damageable);
        }

    }
    
    private bool IsInLayerMask(GameObject go)
    {
        return ((targetLayerMask.value & (1<<go.layer))>0);
    }
}
