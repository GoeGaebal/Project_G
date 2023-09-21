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
    [SerializeField] protected float minDisFromPlayer;
    [SerializeField] private bool isSpriteRightSide;
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
            AnimState.Init();
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

        AnimState = idleState;
        _spawnPosition = transform.position;
    }

    protected virtual void Update() {
        
        if(isDead) return;
        
        Collider2D[] playerColliders = Physics2D.OverlapCircleAll(gameObject.transform.position, detectRadius,chaseTargetLayerMask);
 

        if(!hasTarget)
        {
             if((transform.position - _spawnPosition).magnitude > 1.0f)
            {
                AnimState = runState;
            }

            foreach(var playerCollider in playerColliders)
            {
                if(playerCollider.GetComponent<DamageableEntity>() == null || playerCollider.GetComponent<DamageableEntity>().isDead) continue;
                hasTarget = true;
                target = playerCollider.gameObject;
            }

        }

        if(playerColliders.Length == 0 || playerColliders == null)
        {
            hasTarget = false;          
            target = null;
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
        }
        AnimState.UpdateInState();
        
    }

    [PunRPC]
    override public void OnDamage(float damage) 
    {
        base.OnDamage(damage);
        if(isDead) return;
        if(!(AnimState is IdleState) && !(AnimState is RunState)) return;
        AnimState = hitState;
    }


     protected virtual IEnumerator DieCoroutine()

    {
        animator.ResetTrigger("hit");   
        animator.SetTrigger("die");
        yield return new WaitForSeconds(1.0f);

        Managers.Network.RequestSpawnLootingItems(5001, 5, transform.position, 2.0f, 1.0f);
        Destroy(gameObject);
    }
        protected float GetDistance()
    { 
        return (target.transform.position - transform.position).magnitude;
    }

    public void FinishAttackState()
    {
        AnimState = idleState;
    }

    public void FinishHitState()
    {
        AnimState = idleState;
    }

    protected virtual void DoFlip(bool value)
    {
        if(isSpriteRightSide)spriteRenderer.flipX = value;
        else spriteRenderer.flipX = !value;
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
            if(basicMonster.target != null && basicMonster.GetDistance() > basicMonster.minDisFromPlayer )
            {
                basicMonster.AnimState = basicMonster.runState;
            }
            else if(basicMonster.hasTarget == true && Time.time - basicMonster.lastAttackTime >=basicMonster. attackCooldown)  
            {
                basicMonster.lastAttackTime = Time.time;
                basicMonster.AnimState = basicMonster.attackState;
            }
            if(basicMonster.target != null)
            {
                float distance = basicMonster.GetDistance();
                basicMonster.animator.SetFloat("distance", distance);
            }
            
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
            if(basicMonster.target != null)
            {
                float distance = basicMonster.GetDistance();
                basicMonster.animator.SetFloat("distance", distance);
            }
           
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
            if(!basicMonster.animator.GetBool("run")) basicMonster.animator.SetBool("run",true);
            if(basicMonster.hasTarget == false )
            {
                if(basicMonster._spawnPosition.x < basicMonster.transform.position.x) basicMonster.DoFlip(true);
                else basicMonster.DoFlip(false);
                basicMonster.transform.position = Vector3.MoveTowards(basicMonster.transform.position, basicMonster._spawnPosition, basicMonster.speed * Time.deltaTime);
                
                if( (basicMonster.transform.position - basicMonster._spawnPosition).magnitude <= 0.001f)
                {
                    basicMonster.AnimState = basicMonster.idleState;
                }
            }
            else if (basicMonster.target != null && basicMonster.GetDistance()<= basicMonster.minDisFromPlayer)
            {
                basicMonster.AnimState = basicMonster.idleState;
            }
            else if(basicMonster.target != null && basicMonster.GetDistance()> basicMonster.minDisFromPlayer)
            {
                if(basicMonster.target.transform.position.x < basicMonster.transform.position.x) basicMonster.DoFlip(true);
                else basicMonster.DoFlip(false);
                basicMonster.transform.position = Vector3.MoveTowards(basicMonster.transform.position, basicMonster.target.transform.position, basicMonster.speed * Time.deltaTime);

                if(basicMonster.target != null)
                {
                    float distance = basicMonster.GetDistance();
                    basicMonster.animator.SetFloat("distance", distance);
                }
                
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
    }
}
