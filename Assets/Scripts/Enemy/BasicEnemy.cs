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

    protected bool hasTarget = false;
    protected Vector2 targetPos;

    private void Update() {
        Collider2D playerCollider = Physics2D.OverlapCircle(gameObject.transform.position, detectRadius,targetLayerMask);
        if(playerCollider != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            animator.SetTrigger("triggerRun");
            Vector2 playerpos = playerCollider.transform.position;
            if(!hasTarget)
            {
                hasTarget = true;
                targetPos = playerCollider.transform.position;
            }
        
        }
        else if(playerCollider == null)
        {
            hasTarget = false;
            targetPos = transform.position;
            animator.SetTrigger("triggerIdle");
        }
        if(hasTarget)
        {
            if(targetPos.x < transform.position.x) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;
        }
        
    }
}
