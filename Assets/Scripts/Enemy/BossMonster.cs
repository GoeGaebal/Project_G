using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BossMonster : BasicMonster
{

    protected override void FlipXSprite()
    {
        if(target.transform.position.x < transform.position.x) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;
    }


    protected override void Update() {

     

        base.Update();


    }
}
