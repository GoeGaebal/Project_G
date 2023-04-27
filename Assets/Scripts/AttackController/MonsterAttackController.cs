using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackController : AttackController
{
    private void Start() {
        weaponController = new MonsterWeaponController();
    }
    
}
