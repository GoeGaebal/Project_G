using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasBossEarthquakeAttackController : MonsterAttackController
{
    private void OnEnable() {
        StartCoroutine(DisappearAttackEffect());
    }

    IEnumerator DisappearAttackEffect()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }
}
