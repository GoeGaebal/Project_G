using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : DamageableEntity
{
    [SerializeField] protected float attackPoint; 
    [SerializeField] protected float attackCooldown;
    [SerializeField] private float detectRadius;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] protected Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] protected float speed;

    protected bool hasTarget;
    protected GameObject target;
    protected float lastAttackTime;
   


    private void Start() {
        hasTarget = false;
        target = null;
        lastAttackTime = 0;

        dieAction += ()=> StartCoroutine("DieCoroutine");
    }
    private void Update() {
        Collider2D playerCollider = Physics2D.OverlapCircle(gameObject.transform.position, detectRadius,targetLayerMask);
       
      


        //reset target
        //여긴 문제 없고
        if(playerCollider == null)
        {
            hasTarget = false;
            animator.SetBool("hasTarget",hasTarget);
            target = null;
            
        }

        //set target when collider is detected but no target
        else if(!hasTarget && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
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

            Vector3 playerpos = target.transform.position;
            animator.SetFloat("distance", (playerpos - transform.position).magnitude);
            

            //move
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            }

            else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {   
                //play attack animation
                if(Time.time - lastAttackTime >= attackCooldown) 
                {
                    lastAttackTime = Time.time;
                    animator.SetTrigger("attack");
                    
                }
            }

        }
        
    }
    override public void OnDamage(float damage) 
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            animator.SetTrigger("hit");
        }
        base.OnDamage(damage);
        
    }


    private IEnumerator DieCoroutine()

    {
        animator.ResetTrigger("hit");   
        animator.SetTrigger("die");
        
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
    
}
