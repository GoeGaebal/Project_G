using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackController : AttackController
{
    private void Start() {
        weaponController = new MonsterWeaponController();
    }
    
}

class MonsterWeaponController : Weapon
{
    public MonsterWeaponController()
    {
        Debug.Log("monster weapon controller created");
        AddTargetLayer((int)EnumLayerMask.Player);
    }
}
