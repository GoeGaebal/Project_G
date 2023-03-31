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
            UpdateAnimation();
            
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
        Collider2D playerCollider = Physics2D.OverlapCircle(gameObject.transform.position, detectRadius,targetLayerMask);
 
        if(playerCollider == null)
        {
            hasTarget = false;  
            AnimState = EnumAnimationStates.Idle;
            animator.SetBool("hasTarget",hasTarget);
            target = null;
            
        }

        //set target when collider is detected but no target
        else if(!hasTarget && AnimState == EnumAnimationStates.Idle)
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
        AnimState = EnumAnimationStates.Hit;
        base.OnDamage(damage);
        
    }


    private IEnumerator DieCoroutine()

    {

        animator.ResetTrigger("hit");   
        animator.SetTrigger("die");
        
        
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
    


    private void UpdateAnimation()
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
        
        EnumAnimationStates previousState = AnimState;

        if(previousState == EnumAnimationStates.Idle || previousState == EnumAnimationStates.Run)
        {
            animator.SetTrigger("hit");
            yield return new WaitForSeconds(1.0f);
            AnimState = previousState;
        }
    }
}
