using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxWeaponController : IWeapon
{
    public virtual void Attack(IDamageable damageable)
    {
        return;
    }
}
