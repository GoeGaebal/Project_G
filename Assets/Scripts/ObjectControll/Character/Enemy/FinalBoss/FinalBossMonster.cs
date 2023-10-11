using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public partial class FinalBossMonster : DamageableEntity
{
    [SerializeField] private float attackPeriod;
    private float fromLastAttackTime;
    private List< ICastingSpell> spells = new();

    [SerializeField] private List<GameObject> thunders;

    // Start is called before the first frame update
    void Start()
    {
        fromLastAttackTime = 0f;
        spells.Add(new ThunderSpell(this));
        dieAction += () => StartCoroutine(Diecoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead) return;
        fromLastAttackTime+=Time.deltaTime;
        if(fromLastAttackTime>attackPeriod)
        {
            fromLastAttackTime = 0f;
            int idx = Random.Range(0,spells.Count);
            spells[idx].Spell();
        }
    }

    IEnumerator Diecoroutine()
    {
        yield return new WaitForSeconds(3.0f);

        Destroy(gameObject);
        Managers.Scene.LoadScene(SceneType.Credit);
    }
}
