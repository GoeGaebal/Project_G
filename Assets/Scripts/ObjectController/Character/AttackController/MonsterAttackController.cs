using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackController : AttackController
{
    private void Start() {
        weaponController = new MonsterWeaponController();
    }
    
    public void SetActive(bool active) {
        gameObject.SetActive(active);
    }
}

class MonsterWeaponController : Weapon
{
    public MonsterWeaponController()
    {
        AddTargetLayer((int)EnumLayerMask.Player);
    }
}
