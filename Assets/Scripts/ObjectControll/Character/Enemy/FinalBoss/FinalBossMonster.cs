using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FinalBossMonster : DamageableEntity
{
    [SerializeField] private float attackPeriod;
    private float fromLastAttackTime;
    private List< ICastingSpell> spells = new();

    [SerializeField] private List<GameObject> thunders;
    private Renderer dissolveMat;
    private float dissolveSpeed = 0.002f;

    // Start is called before the first frame update
    void Start()
    {
        fromLastAttackTime = 0f;
        spells.Add(new ThunderSpell(this));
        dieAction += () => StartCoroutine(Diecoroutine());
        dissolveMat = GetComponent<Renderer>();
        dissolveMat.material.SetFloat("_DissolveStep",1.4f);
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

        while(dissolveMat.material.GetFloat("_DissolveStep") >= -0.2f)
        {
            dissolveMat.material.SetFloat("_DissolveStep", dissolveMat.material.GetFloat("_DissolveStep") - dissolveSpeed);
            yield return null; 
        }

        yield return new WaitForSeconds(3.0f);

        Destroy(gameObject);
        Managers.Scene.LoadScene(Define.Scene.Credit);
    }
}
