using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossAttackController : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField]private float _damage = 10f;
    public float damage{
        get{return _damage;}
        set{_damage = value;}
    }
   
    private void OnTriggerEnter2D(Collider2D other) {
        if((1<<other.gameObject.layer & targetLayerMask.value) > 0)
        {
            DamageableEntity damageableEntity = other.GetComponent<DamageableEntity>();
            if(damageableEntity == null) return;
            damageableEntity.OnDamage(damage);
        }
    }

    public void UnActivation()
    {   
        gameObject.SetActive(false);
    }
}
