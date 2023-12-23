using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public interface IDamageable
{
    void OnDamage(float damage);
    void UpdateHp(float hp, bool isDead);
    void OnHit(CreatureState state);
}

