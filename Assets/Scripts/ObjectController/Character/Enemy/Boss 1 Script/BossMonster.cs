using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BossMonster : BasicMonster
{   
    public static Action FinishSpellAction;
    [SerializeField] private Vector2 BossroomLeftDownWorldPos;
    [SerializeField] private Vector2 BossroomRightUpWorldPos;
    private CastingState castingState;
    private List<ICastingSpell> spells = new();
    private List<Transform> thunders = new();
    public List<Transform> Thunders
    {
        get{return thunders;}
    }
    bool CanCastingState = true;
     protected override void Start() {
        base.Start();
        
        runState = new BossRunState(this);

        FinishSpellAction = () =>{FinishSpell();}; 

        castingState = new CastingState(this);
        spells.Add(new NEWSSpell(this));
        spells.Add(new TeleportTargetSpell(this));

        for(int i = 0;i< 4;i++)
        {
            Debug.Log("BossThunder"+i.ToString());
            thunders.Add(transform.Find(string.Format("BossThunder{0}",i)));
            
        }
    }

    [SerializeField] float cooltimeCasting;

    protected override void Update() {
        base.Update();

        if(target==null) return;
        if(target.GetComponent<DamageableEntity>().isDead) return;
        if(isDead) return;
        if(hasTarget == false || target == null) return;
        
        if(CanCastingState && (AnimState is RunState || AnimState is IdleState)) AnimState = castingState;
    }

    protected override void DoFlip(bool value)
    {
        base.DoFlip(value);
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
        spells[UnityEngine.Random.Range(0,spells.Count)].Spell();
    }
    public void FinishCasthingState()
    {
        AnimState = this.idleState;
    }

    private IEnumerator CooldownTimeCastingState()
    {
        CanCastingState = false;

        yield return new WaitForSeconds(cooltimeCasting);
        CanCastingState = true;
    }

    protected override IEnumerator DieCoroutine()
    {
        animator.ResetTrigger("hit");   
        animator.SetTrigger("die");
        yield return new WaitForSeconds(1.0f);

        //여기에 아티팩트 드랍 코드 넣으면 됨
        Managers.Network.RequestSpawnLootingItems(5101, 10, transform.position, 2.0f, 1.0f);
        Managers.Network.RequestSpawnLootingItems(5001, 5, transform.position, 2.0f, 1.0f);
        Destroy(gameObject);
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



    class NEWSSpell: ICastingSpell
    {
        BossMonster _bossMonster;
        public NEWSSpell(BossMonster bm)
        {
            _bossMonster = bm;
        }
        public void Spell()
        {
            Debug.Log("do spell");
            List<Transform> tfs = _bossMonster.Thunders;
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

    class TeleportTargetSpell:ICastingSpell
    {
        BossMonster _bossMonster;
        public TeleportTargetSpell(BossMonster bm)
        {
            _bossMonster = bm;
        }
        public void Spell()
        {
            if (_bossMonster.Target == null)
            {
                return;
            }
            _bossMonster.Target.transform.position = (Vector2)_bossMonster.transform.position + Vector2.right * 0.5f;
        }

    }

    protected class BossRunState:RunState
    {
        BossMonster _bossMonster;
        internal BossRunState(BossMonster bm) : base(bm)
        {
            _bossMonster = bm;
        }
        public override void Init()
        {
            base.Init();
        }
        public override void UpdateInState()
        {
            base.UpdateInState();
            float xpos = Mathf.Clamp(basicMonster.transform.position.x, _bossMonster.BossroomLeftDownWorldPos.x, _bossMonster.BossroomRightUpWorldPos.x);
            float ypos = Mathf.Clamp(basicMonster.transform.position.y, _bossMonster.BossroomLeftDownWorldPos.y, _bossMonster.BossroomRightUpWorldPos.y);

            basicMonster.transform.position = new Vector3(xpos,ypos,basicMonster.transform.position.z);
        }
    }
}
