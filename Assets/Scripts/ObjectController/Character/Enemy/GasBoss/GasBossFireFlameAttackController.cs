using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasBossFireFlameAttackController : MonsterAttackController
{
     private void OnEnable() {
        StartCoroutine(DisappearAttackEffect());
    }

    IEnumerator DisappearAttackEffect()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
