using System;
using Google.Protobuf.Protocol;

public abstract class CreatureController : NetworkObject
{
    public float maxHP;
    public float HP {get; protected set;} // 기존의 getter, setter 메소드를 대체한다, get은 public , set은 protected으로 접근제어를 한다.
    
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

    private void UpdateState()
    {
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

    protected virtual void OnDie()
    {
        if (Managers.Network.IsHost)
        {
            S_DeSpawn despawn = new S_DeSpawn();
            despawn.ObjectIds.Add(Id);
            Managers.Network.Server.Room.Broadcast(despawn);
            Managers.Network.Server.Room.SpawnLootingItems(5001,5,transform.position,2.0f, 1.0f);
        }
    }
    protected abstract void OnAttack();
    protected abstract void OnHit();
    
    public virtual void OnDamage(float damage)
    {
        if (Managers.Network.IsHost)
        {
            var hp = HP;
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
        OnHit();
    }

    public void UpdateHP(float hp, bool dead) 
    {
        HP = hp;
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

    public override void SyncPos()
    {
        base.SyncPos();
        State = PosInfo.State;
    }
}
