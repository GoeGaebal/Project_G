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
    }
    private void Update() {
        Collider2D playerCollider = Physics2D.OverlapCircle(gameObject.transform.position, detectRadius,targetLayerMask);
       
        //타겟 설정
        // overlapCircle로 탐지된 collider가 없으면 그냥 넘김.
        if(playerCollider == null)
        {
            hasTarget = false;
            animator.SetBool("hasTarget",hasTarget);
            target = null;
            
        }

        //collider 탐지되었는데 target이 없으면 target 새로 설정
        else if(!target && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            hasTarget = true;
            animator.SetBool("hasTarget", hasTarget);
            target = playerCollider.gameObject;

        
        }
        
        if(hasTarget)
        {
            //target check
            if(target == null) return;

            if(target.transform.position.x < transform.position.x) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;

            Vector3 playerpos = target.transform.position;
            animator.SetFloat("distance", (playerpos - transform.position).magnitude);
            
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            }

            else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                if(Time.time - lastAttackTime >= attackCooldown) 
                {
                    lastAttackTime = Time.time;
                    animator.SetTrigger("attack");
                }
            }

        }
        
    }

    
}
