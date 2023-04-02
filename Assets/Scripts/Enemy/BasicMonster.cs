using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumAnimationStates{
    Idle, Run, Attack, Hit
};

public class BasicMonster : DamageableEntity
{
    [SerializeField] protected float attackPoint; 
    [SerializeField] protected float attackCooldown;
    [SerializeField] private float detectRadius;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] protected float speed;
    [SerializeField] private float minDisFromPlayer;

    protected Animator animator;
    private SpriteRenderer spriteRenderer;



    protected bool hasTarget;
    protected GameObject target;
    protected float lastAttackTime;
    private EnumAnimationStates _animState;
    public EnumAnimationStates AnimState
    {
        get{
            return _animState;
        }

        protected set{ 
            _animState = value; 
            UpdateState();
            
        }

    }


    private void Start() {

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        hasTarget = false;
        target = null;
        lastAttackTime = 0;
        AnimState = EnumAnimationStates.Idle;
        dieAction += ()=> StartCoroutine("DieCoroutine");

        
    }

    private void Update() {
        
        if(isDead) return;

        Collider2D playerCollider = Physics2D.OverlapCircle(gameObject.transform.position, detectRadius,targetLayerMask);
 
        if(playerCollider == null)
        {
            hasTarget = false;  
            if( AnimState != EnumAnimationStates.Attack ||AnimState != EnumAnimationStates.Hit )    
                AnimState = EnumAnimationStates.Idle;
            
            animator.SetBool("hasTarget",hasTarget);
            target = null;
            
        }

        //set target when collider is detected but no target
        else if(!hasTarget)
        {
            hasTarget = true;
            animator.SetBool("hasTarget", hasTarget);
            target = playerCollider.gameObject;
        }

        
        if(hasTarget)
        {
            
            //target check
            if(target == null) return;

            //flip sprite
            if(target.transform.position.x < transform.position.x) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;

            float distance = (target.transform.position - transform.position).magnitude;
            animator.SetFloat("distance", distance);

            if(AnimState == EnumAnimationStates.Idle && distance > minDisFromPlayer )
            {
                AnimState = EnumAnimationStates.Run;

            }
            else if (AnimState == EnumAnimationStates.Run && distance<= minDisFromPlayer)
            {
                AnimState = EnumAnimationStates.Idle;
            }

            
 
            
        }
        
    }

    private void LateUpdate() {

        if(isDead) return;
        
        if(hasTarget)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            switch(AnimState)
                {
                    case EnumAnimationStates.Run:
                        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                        break;

                    case EnumAnimationStates.Idle:
                        if(Time.time - lastAttackTime >= attackCooldown && distance <= minDisFromPlayer)  
                        {
                            lastAttackTime = Time.time;
                            AnimState = EnumAnimationStates.Attack;
                        }
                        break;
                    default:
                        break;
            }
        }
    }
    override public void OnDamage(float damage) 
    {
        
        base.OnDamage(damage);
        if(!isDead)
            AnimState = EnumAnimationStates.Hit;
        
    }


   
    


    private void UpdateState()
    {
        switch (AnimState)
        {
            case EnumAnimationStates.Idle:
                animator.SetBool("run",false);
                break;

            case EnumAnimationStates.Run:
                animator.SetBool("run",true);
                break;
            case EnumAnimationStates.Attack:
                StartCoroutine("AttackCoroutine");
                break;
            case EnumAnimationStates.Hit:
                StartCoroutine("HitCoroutine");
                break;
            default:
                break;
        }
    }


    private IEnumerator AttackCoroutine()
    {
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(1.0f);
        AnimState = EnumAnimationStates.Idle;
    }

    private IEnumerator HitCoroutine()
    {
        
       
            animator.SetTrigger("hit");
            yield return new WaitForSeconds(2.0f);

            if(target == null) yield break;
        
            float distance = (target.transform.position - transform.position).magnitude;
            if(hasTarget && distance > minDisFromPlayer) AnimState = EnumAnimationStates.Run;
            else AnimState = EnumAnimationStates.Idle;
    }

     private IEnumerator DieCoroutine()

    {
        animator.ResetTrigger("hit");   
        animator.SetTrigger("die");
        
        
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
