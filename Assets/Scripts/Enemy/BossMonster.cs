using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BossMonster : BasicMonster
{   
    CastingState castingState;
    bool CanCastingState = true;
     protected override void Start() {
        base.Start();

        castingState = new CastingState(this);
    }

    [SerializeField] float cooltimeCasting;

    protected override void Update() {

        base.Update();

        if(isDead) return;
        if(hasTarget == false || target == null) return;

        if(CanCastingState && (AnimState is RunState || AnimState is IdleState)) ChangeState(castingState);


        
    }


    protected override void FlipXSprite()
    {
        if(target.transform.position.x < transform.position.x) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;
    }

    private IEnumerator CooldownTimeCastingState()
    {
        CanCastingState = false;

        yield return new WaitForSeconds(cooltimeCasting);
        CanCastingState = true;
    }

    protected class CastingState:State{
        internal CastingState(BasicMonster bm){
            basicMonster = bm;
        }
        public override void Init()
        {
            Debug.Log("casting");
            basicMonster.StartCoroutine("CooldownTimeCastingState");
            basicMonster.animator.SetTrigger("casting");
        }

        public override void UpdateInState()
        {

        }
    }
}
