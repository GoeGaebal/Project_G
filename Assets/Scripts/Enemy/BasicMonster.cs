using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumAnimationState{
    Idle, Run
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
    private EnumAnimationState _animState;
    public EnumAnimationState AnimState
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
        hasTarget = false;
        target = null;
        lastAttackTime = 0;
        AnimState = EnumAnimationState.Idle;
        dieAction += ()=> StartCoroutine("DieCoroutine");

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        Collider2D playerCollider = Physics2D.OverlapCircle(gameObject.transform.position, detectRadius,targetLayerMask);
 
        if(playerCollider == null)
        {
            hasTarget = false;  
            AnimState = EnumAnimationState.Idle;
            animator.SetBool("hasTarget",hasTarget);
            target = null;
            
        }

        //set target when collider is detected but no target
        else if(!hasTarget && AnimState == EnumAnimationState.Idle)
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

            if(AnimState == EnumAnimationState.Idle && distance > minDisFromPlayer )
            {
                AnimState = EnumAnimationState.Run;

            }
            else if (AnimState == EnumAnimationState.Run && distance<= minDisFromPlayer)
            {
                AnimState = EnumAnimationState.Idle;
            }

            
 
            switch(AnimState)
            {
                case EnumAnimationState.Run:
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                    break;

                case EnumAnimationState.Idle:
                    if(Time.time - lastAttackTime >= attackCooldown && distance <= minDisFromPlayer)  
                    {
                        lastAttackTime = Time.time;
                        animator.SetTrigger("attack");
                        
                    }
                    break;
                default:
                    break;
            }
        }
        
    }
    override public void OnDamage(float damage) 
    {
        animator.SetTrigger("hit");
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
            case EnumAnimationState.Idle:
                animator.SetBool("run",false);
                break;

            case EnumAnimationState.Run:
                animator.SetBool("run",true);
                break;
            default:
                break;
        }
    }
}
