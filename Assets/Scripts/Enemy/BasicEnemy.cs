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

    protected bool hasTarget = false;
    protected GameObject target = null;

    private void Update() {
        Collider2D playerCollider = Physics2D.OverlapCircle(gameObject.transform.position, detectRadius,targetLayerMask);

        
        //타겟 설정
        if(playerCollider != null && target == null && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            hasTarget = true;
            animator.SetBool("hasTarget", hasTarget);
            target = playerCollider.gameObject;

            Vector3 playerpos = target.transform.position;
            animator.SetFloat("distance", (playerpos - transform.position).magnitude);

            Debug.Log(animator.GetBool("hasTarget"));
            Debug.Log(animator.GetFloat("distance"));
        
        }
        else if(playerCollider == null)
        {
            hasTarget = false;
            animator.SetBool("hasTarget",hasTarget);
            target = null;
            
        }
        if(hasTarget)
        {
            //target
            if(target.transform.position.x < transform.position.x) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;

            Vector3 playerpos = target.transform.position;
            animator.SetFloat("distance", (playerpos - transform.position).magnitude);
            Debug.Log(animator.GetFloat("distance"));

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                }

        }
        
    }
}
