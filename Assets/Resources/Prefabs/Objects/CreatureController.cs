using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CreatureController : NetworkObject
{
    public float maxHP;
    public float HP{get;protected set;} // 기존의 getter, setter 메소드를 대체한다, get은 public , set은 protected으로 접근제어를 한다.
    
    private CreatureState _state;
    public bool IsDead => CreatureState.Dead == _state;
    public CreatureState State
    {
        get => _state;
        protected set
        {
            _state = value;
            UpdateState();
        }
    }
    
    protected virtual void UpdateState()
    {
        if(IsDead) return;
        switch (State)
        {
            case CreatureState.Idle:
                OnIdle();
                break;
            case CreatureState.Run:
                OnRun();
                break;
            case CreatureState.Dead:
                OnDie();
                break;
            case CreatureState.Attack:
                OnAttack();
                break;
            case CreatureState.Hit:
                OnHit();
                break;
            default:
                break;
        }
    }

    protected abstract void OnIdle();
    protected abstract void OnRun();
    protected abstract void OnDie();
    protected abstract void OnAttack();
    protected abstract void OnHit();
    
    public virtual void OnDamage(float damage)
    {
        if (IsDead) return;
        if(Managers.Network.isHost)
        {
            HP -= damage;
            S_OnDamage packet = new S_OnDamage();
            packet.ObjectId = Id;
            packet.Damage = damage;
            packet.HP = HP;
            packet.IsDead = IsDead;
            UpdateHP(HP, IsDead);
            Managers.Network.Server.Room.Broadcast(packet);
        }
        
        if (HP<=0 && !IsDead)
        {
            Die();
        }
    }
    
    public virtual void Die()
    {
        if (IsDead) return;
        State = CreatureState.Dead;
        Debug.Log("Die");
        
    }
    
    public virtual void UpdateHP(float health, bool dead) 
    {
        this.HP = health;
        if (dead) State = CreatureState.Dead;
    }

    protected virtual void OnEnable() { // 객체가 enable 될 때 호출되는 함수
        // 체력이랑 isDead 변수 초기화, 살아 있는 상태로 만든다.
        State = CreatureState.Idle;
        HP = maxHP;
    }
    
    public void Revive(float heal)
    {
        if(!IsDead) return;
        State = CreatureState.Idle;
        RestoreHP(heal);
    }
    
    public virtual void RestoreHP(float restoreHP)
    {
        if (IsDead) return;     

        if (HP + restoreHP >= maxHP) HP = maxHP;
        else HP += restoreHP;
    }
}
