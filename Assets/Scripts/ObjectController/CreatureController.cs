using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;

public abstract class CreatureController : NetworkObject, IDamageable
{
    private CreatureState _state;
    public bool IsDead => CreatureState.Dead == _state;
    protected Dictionary<CreatureState, Action<CreatureState>> UpdateState = new();
    public float HP {
        get => StatInfo.Hp;
        private set => StatInfo.Hp = value;
    }
    public float maxHP => StatInfo.MaxHp;

    protected CreatureState State
    {
        get => _state;
        set
        {
            if (IsDead) return;
            UpdateState[value]?.Invoke(_state);
            _state = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        StatInfo.MaxHp = 100;
        StatInfo.Hp = StatInfo.MaxHp;
        UpdateState.Clear();
        UpdateState.Add(CreatureState.Idle, OnIdle);
        UpdateState.Add(CreatureState.Dead, OnDie);
        UpdateState.Add(CreatureState.Hit, OnHit);
    }

    protected abstract void OnIdle(CreatureState state);

    protected virtual void OnDie(CreatureState state)
    {
        if (state == CreatureState.Dead) return;
    }

    public virtual void OnDamage(float damage)
    {
        if (State == CreatureState.Hit) return;
        if (Managers.Network.IsHost)
        {
            var hp = StatInfo.Hp;
            hp -= damage;
            var packet = new S_OnDamage
            {
                ObjectId = Id,
                Damage = damage,
                HP = hp,
                IsDead = hp <= 0
            };
            Managers.Network.Server.Room.Broadcast(packet);
        }
        State = CreatureState.Hit;
    }

    public abstract void OnHit(CreatureState state);

    public virtual void UpdateHp(float hp, bool dead) 
    {
        StatInfo.Hp = hp;
        if (dead) State = CreatureState.Dead;
    }

    protected virtual void OnEnable() { // 객체가 enable 될 때 호출되는 함수
        // 체력이랑 isDead 변수 초기화, 살아 있는 상태로 만든다.
        State = CreatureState.Idle;
        StatInfo.Hp = StatInfo.MaxHp;
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

    public override void SyncPos()
    {
        base.SyncPos();
        State = PosInfo.State;
    }
}
