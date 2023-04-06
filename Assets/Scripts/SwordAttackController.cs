using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttackController : IWeapon
{
    public virtual void Attack(Collider2D other)
    {
        BasicMonster monster = other.GetComponent<BasicMonster>();

        if(monster == null) return;

        monster.OnDamage(10.0f);
    }
}
