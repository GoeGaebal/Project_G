using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : IWeapon
{
    public virtual void Attack(IDamageable damageable)
    {
       
        damageable.OnDamage(10.0f);
    }
}
