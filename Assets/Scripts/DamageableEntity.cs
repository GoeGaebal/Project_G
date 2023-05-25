using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class DamageableEntity : MonoBehaviourPun, IDamageable
{
    public float maxHP;
    public float HP{get;protected set;} // 기존의 getter, setter 메소드를 대체한다, get은 public , set은 protected으로 접근제어를 한다.

    public bool isDead {get;protected set;}

    // event는 Action이 이 클래스 외부에서 실행되는 것을 방지한다.
    // 그리고 Ondeath가 호출되어야 하는 시점에 어떤 행동을 해야 하는지 런타임 단계에서 정할 수 있다.
    public event Action dieAction; 

    protected virtual void OnEnable() { // 객체가 enable 될 때 호출되는 함수
        // 체력이랑 isDead 변수 초기화, 살아 있는 상태로 만든다.
        isDead = false;
        HP = maxHP;
    }

    ///<summary>
    ///  적이 데미지를 입었을 때 호출되는 함수
    ///</summary>
    [PunRPC]
    public virtual void OnDamage(float damage)
    {
        if (isDead) return;
        if(PhotonNetwork.IsMasterClient)
        {
            HP -= damage;
            photonView.RPC("UpdateHP",RpcTarget.Others,HP, isDead);
            photonView.RPC("OnDamage",RpcTarget.Others,damage);
        }
        
        if (HP<=0 && !isDead)
        {
            Die();
        }
    }
    [PunRPC]
    public void UpdateHP(float health, bool dead) 
    {
        this.HP = health;
        this.isDead = dead;
    }
    
    public virtual void RestoreHP(float restoreHP)
    {
        if (isDead) return;     

        if (HP + restoreHP >= maxHP) HP = maxHP;
        else HP += restoreHP;
       

    }

    /// <summary>
    /// 적이 죽었을 때 호출되는 함수
    /// </summary>
    public virtual void Die()
    { 
        if (isDead) return;
        isDead = true;

        if(dieAction != null)
        {
            Debug.Log("die action");
            dieAction();  
        }
        Debug.Log("Die");
        
    }
}
