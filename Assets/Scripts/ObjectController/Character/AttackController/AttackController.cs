using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumLayerMask
{
    Player = 6, Monster = 3, Mineral = 8
};

public class AttackController : MonoBehaviour
{
    [SerializeField] protected LayerMask targetLayerMask;
    [SerializeField] protected float damage = 20f;

    protected virtual void OnTriggerEnter2D(Collider2D other) {

        if(other == null) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable == null) return;
        if(CheckAttackLayer((int)other.gameObject.layer))
        {
            damageable.OnDamage(damage);
        }

    }

    protected bool CheckAttackLayer(int attackLayer)
    {
        return ((targetLayerMask & (1<<attackLayer))>0);
    }

    protected void AddTargetLayer(int layerNum)
    {
        targetLayerMask |= (1<<layerNum);
    }
    protected void RemoveTargetLayer(int layerNum)
    {
        targetLayerMask &=  ~(1<<layerNum);
    }

    protected void ResetTargetLayer()
    {
        targetLayerMask = 0;
    }
}
