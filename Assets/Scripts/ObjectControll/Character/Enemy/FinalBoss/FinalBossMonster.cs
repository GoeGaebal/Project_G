using System.Collections;
using System.Collections.Generic;
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
    }

    // Update is called once per frame
    void Update()
    {
        fromLastAttackTime+=Time.deltaTime;
        if(fromLastAttackTime>attackPeriod)
        {
            fromLastAttackTime = 0f;
            int idx = Random.Range(0,spells.Count);
            spells[idx].Spell();
        }
    }
}
