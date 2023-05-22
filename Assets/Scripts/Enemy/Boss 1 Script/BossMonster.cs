using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BossMonster : BasicMonster
{   
    public static Action FinishSpellAction;

    private CastingState castingState;
    private List<CastingSpell> spells = new();
    private List<Transform> thunders = new();
    public List<Transform> Thunders
    {
        get{return thunders;}
    }
    bool CanCastingState = true;
     protected override void Start() {
        base.Start();
        
        FinishSpellAction = () =>{FinishSpell();}; 

        castingState = new CastingState(this);
        spells.Add(new NEWSSpell());
        spells.Add(new TeleportTargetSpell());

        for(int i = 0;i< 4;i++)
        {
            Debug.Log("BossThunder"+i.ToString());
            thunders.Add(transform.Find(string.Format("BossThunder{0}",i)));
            
        }
    }

    [SerializeField] float cooltimeCasting;

    protected override void Update() {
        base.Update();

        if(isDead) return;
        if(hasTarget == false || target == null) return;

        if(CanCastingState && (AnimState is RunState || AnimState is IdleState)) ChangeState(castingState);


        
    }
    public void FinishSpell()
    {
        foreach(var thunder in thunders)
        {
            thunder.SetParent(transform);
            thunder.gameObject.SetActive(false);
        }
    }

    public void DoSpell()
    {
        spells[UnityEngine.Random.Range(0,spells.Count)].Spell(this);
    }
    public void FinishCasthingState()
    {
        ChangeState(this.idleState);
    }
    protected override void FlipXSprite()
    {
        if(target.transform.position.x < transform.position.x) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;
    }

    private IEnumerator CooldownTimeCastingState()
    {
        CanCastingState = false;

        yield return new WaitForSeconds(cooltimeCasting);
        CanCastingState = true;
    }

    protected class CastingState:State{
        internal CastingState(BasicMonster bm){
            basicMonster = bm;
        }
        public override void Init()
        {
            Debug.Log("casting");
            basicMonster.StartCoroutine("CooldownTimeCastingState");
            basicMonster.animator.SetTrigger("casting");
        }

        public override void UpdateInState()
        {
        }
    }

    interface CastingSpell
    {
        void Spell(BossMonster bm);
    }

    class NEWSSpell: CastingSpell
    {
        public void Spell(BossMonster bm)
        {
            Debug.Log("do spell");
            List<Transform> tfs = bm.Thunders;
            foreach(Transform tf in tfs)
            {
                tf.gameObject.SetActive(true);
            }
            tfs[0].localPosition = new Vector2(0f,0.5f);
            tfs[1].localPosition = new Vector2(0.5f,0f);
            tfs[2].localPosition = new Vector2(0f,-0.5f);
            tfs[3].localPosition = new Vector2(-0.5f,0f);

            foreach(Transform tf in tfs)
            {
                tf.SetParent(null);
            }
            
            
        }
    }

    class TeleportTargetSpell:CastingSpell
    {
        public void Spell(BossMonster bs)
        {
            if (bs.Target == null)
            {
                return;
            }
            bs.Target.transform.position = (Vector2)bs.transform.position + Vector2.right*0.5f;
           
        }
    }
}
