using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackController : AttackController
{
    private void Start() {
        AddTargetLayer((int)EnumLayerMask.Player);
    }
    
    public void SetActive(bool active) {
        gameObject.SetActive(active);
    }
}


