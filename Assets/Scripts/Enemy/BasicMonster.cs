using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;





    public class BasicMonster : DamageableEntity
{
    [SerializeField] protected float attackPoint; 
    [SerializeField] protected float attackCooldown;
    [SerializeField] private float detectRadius;
    [SerializeField] private LayerMask chaseTargetLayerMask;
    [SerializeField] protected float speed;
    [SerializeField] private float minDisFromPlayer;
    internal Vector3 _spawnPosition;


    internal Animator animator;
    protected SpriteRenderer spriteRenderer;

    protected IdleState idleState;
    protected RunState runState;
    protected HitState hitState;
    protected AttackState attackState;

    protected bool hasTarget;
    public bool HasTarget{
        get{return hasTarget;}
    }
    protected GameObject target;
    public GameObject Target
    {
        get{return target;}
    }
    protected float lastAttackTime;
    private State _animState;
    public State AnimState
    {
        get{
            return _animState;
        }

        protected set{ 
            _animState = value; 
            
        }

    }

   
    protected virtual void Start() {

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        hasTarget = false;
        target = null;
        lastAttackTime = 0;
        
        dieAction += ()=> StartCoroutine(DieCoroutine());

        idleState = new (this);
        runState = new(this);
        attackState = new(this);
        hitState = new(this);

        ChangeState(idleState);
        _spawnPosition = transform.position;
    }

    protected virtual void Update() {
        
        if(isDead) return;

        Collider2D[] playerColliders = Physics2D.OverlapCircleAll(gameObject.transform.position, detectRadius,chaseTargetLayerMask);
 

        if(!hasTarget)
        {
             if((transform.position - _spawnPosition).magnitude > 1.0f)
            {
                ChangeState(runState);
                AnimState.UpdateInState();
            }
        }

        if(playerColliders.Length == 0)
        {
            hasTarget = false;  
            if( !(AnimState is  AttackState) || !(AnimState is  HitState ))
                ChangeState(idleState);
            
            target = null;
            
        }

        //set target when collider is detected but no target
        else if(!hasTarget)
        {
            foreach(var playerCollider in playerColliders)
            {
                if(playerCollider.GetComponent<DamageableEntity>() == null || playerCollider.GetComponent<DamageableEntity>().isDead) continue;
                hasTarget = true;
                target = playerCollider.gameObject;
                return;
            }

        }

        
        if(hasTarget)
        {
            
            //target check
            if(target == null) return;
            //타겟 잃어버림
            if(target.GetComponent<DamageableEntity>() == null ||target.GetComponent<DamageableEntity>().isDead) 
            {
                target = null;
                hasTarget = false;
            }
            else
                AnimState.UpdateInState();
            
            
        }
        
    }

    [PunRPC]
    override public void OnDamage(float damage) 
    {
        
        base.OnDamage(damage);
        if(isDead) return;
        if(!(AnimState is IdleState) && !(AnimState is RunState)) return;
        ChangeState(hitState);
        
    }


     private IEnumerator DieCoroutine()

    {
        animator.ResetTrigger("hit");   
        animator.SetTrigger("die");
        
        
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
        protected float GetDistance()
    { 
        return (target.transform.position - transform.position).magnitude;
    }

    public void FinishAttackState()
    {
        ChangeState(idleState);
    }

    public void FinishHitState()
    {
        ChangeState(idleState);
    }

    protected virtual void FlipXSprite()
    {
        if(target.transform.position.x < transform.position.x) spriteRenderer.flipX = true;
        else spriteRenderer.flipX = false;

    }

    public abstract class State
    {
        protected BasicMonster basicMonster;
        public abstract void Init();
        public virtual void UpdateInState()
        {
        }
    }

     protected class IdleState : State
    {
        internal IdleState(BasicMonster bm)
        {
            basicMonster = bm;
        }
        public override void Init()
        {
            basicMonster.animator.SetBool("run",false);
        }

        public override void UpdateInState()
        {
            if(basicMonster.hasTarget) basicMonster.FlipXSprite();


            if(basicMonster.hasTarget ==false && (basicMonster.transform.position - basicMonster._spawnPosition).magnitude > 1.0f)
            {
                basicMonster.ChangeState(basicMonster.runState);
                return;
            }

            if(basicMonster.hasTarget == true && basicMonster.GetDistance() > basicMonster.minDisFromPlayer )
            {
                basicMonster.ChangeState(basicMonster.runState);
            }
            else if(basicMonster.hasTarget == true && Time.time - basicMonster.lastAttackTime >=basicMonster. attackCooldown)  
            {
                basicMonster.lastAttackTime = Time.time;
                basicMonster.ChangeState(basicMonster.attackState);
            }
            float distance = basicMonster.GetDistance();
            basicMonster.animator.SetFloat("distance", distance);
        }
    }

    protected class AttackState : State
    {
        internal AttackState(BasicMonster bm)
        {
            basicMonster = bm;
        }
        public override void Init()
        {
            
            basicMonster.animator.SetTrigger("attack");
        }

        public override void UpdateInState()
        {
            float distance = basicMonster.GetDistance();
            basicMonster.animator.SetFloat("distance", distance);
        }
    }

    protected class RunState : State
    {
        internal RunState(BasicMonster bm)
        {
            basicMonster = bm;
        }
        public override void Init()
        {
            basicMonster.animator.SetBool("run",true);
        }

        public override void UpdateInState()
        {
            if(basicMonster.hasTarget) basicMonster.FlipXSprite();
            if(basicMonster.hasTarget == false )
            {
                Debug.Log((basicMonster.transform.position - basicMonster._spawnPosition).magnitude );
                if( (basicMonster.transform.position - basicMonster._spawnPosition).magnitude > 1.0f)
                {
                    basicMonster.transform.position = Vector2.MoveTowards(basicMonster.transform.position, basicMonster._spawnPosition, basicMonster.speed * Time.deltaTime);
                }
                else basicMonster.ChangeState(basicMonster.idleState);
            }
            else if (basicMonster.GetDistance()<= basicMonster.minDisFromPlayer)
            {
                basicMonster.ChangeState(basicMonster.idleState);
            }
            else
            {
                basicMonster.transform.position = Vector2.MoveTowards(basicMonster.transform.position, basicMonster.target.transform.position, basicMonster.speed * Time.deltaTime);
        
            float distance = basicMonster.GetDistance();
            basicMonster.animator.SetFloat("distance", distance);
            }
                
        }
    }

    protected class HitState : State
    {
        internal HitState (BasicMonster bm)
        {
            basicMonster = bm;
        }
        public override void Init()
        {
            basicMonster.animator.SetTrigger("hit");
        }

        public override void UpdateInState()
        {

        }
    }




    protected void ChangeState(State newState) {

        if(newState is RunState)
            StartCoroutine(DelayRun());
        AnimState = newState;
        AnimState.Init();


        IEnumerator DelayRun()
        {
            yield return new WaitForSeconds(0.4f);
        }
    }
}



