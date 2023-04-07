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

     private void UpdateState()
    {
        if (isDead) return;

        switch (AnimState)
        {
            case EnumAnimationStates.Idle:

                animator.SetBool("run",false);
                break;

            case EnumAnimationStates.Run:
                animator.SetBool("run",true);
                break;
            case EnumAnimationStates.Attack:
                animator.SetTrigger("attack");
                break;
            case EnumAnimationStates.Hit:
                animator.SetTrigger("hit");
                break;
            default:
                break;
        }
    }
    private void Start() {

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        hasTarget = false;
        target = null;
        lastAttackTime = 0;
        AnimState = EnumAnimationStates.Idle;
        dieAction += ()=> StartCoroutine(DieCoroutine());

        
    }

    private void Update() {
        
        if(isDead) return;

        Collider2D playerCollider = Physics2D.OverlapCircle(gameObject.transform.position, detectRadius,targetLayerMask);
 
        if(playerCollider == null)
        {
            hasTarget = false;  
            if( AnimState != EnumAnimationStates.Attack || AnimState != EnumAnimationStates.Hit )    
                AnimState = EnumAnimationStates.Idle;
            
            target = null;
            
        }

        //set target when collider is detected but no target
        else if(!hasTarget)
        {
            hasTarget = true;
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


            switch(AnimState){
                case EnumAnimationStates.Idle:
                    if(distance > minDisFromPlayer )
                    {
                        AnimState = EnumAnimationStates.Run;
                    }
                    else if(Time.time - lastAttackTime >= attackCooldown)  
                    {
                        lastAttackTime = Time.time;
                        AnimState = EnumAnimationStates.Attack;
                    }
                    break;
                case EnumAnimationStates.Run:
                    if (distance<= minDisFromPlayer)
                    {
                        AnimState = EnumAnimationStates.Idle;
                    }
                    else
                        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                    break;
                default:
                    break;
            }         
 
            
        }
        
    }


    override public void OnDamage(float damage) 
    {
        
        base.OnDamage(damage);
        if(isDead) return;
        if(AnimState != EnumAnimationStates.Idle && AnimState != EnumAnimationStates.Run) return;
        AnimState = EnumAnimationStates.Hit;
        
    }


     private IEnumerator DieCoroutine()

    {
        animator.ResetTrigger("hit");   
        animator.SetTrigger("die");
        
        
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

    public void FinishAttackState()
    {

        AnimState = EnumAnimationStates.Idle;
    }

    public void FinishHitState()
    {
        AnimState = EnumAnimationStates.Idle;
    }
}
