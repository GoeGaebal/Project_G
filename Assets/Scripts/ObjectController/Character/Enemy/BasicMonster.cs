using System;
using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class BasicMonster : CreatureController, IAttackable, IMoveable
{
    [Space]
    [SerializeField] protected float attackPoint; 
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float detectRadius;
    [SerializeField] protected LayerMask chaseTargetLayerMask;
    [SerializeField] protected float speed;
    [SerializeField] protected float minDisFromPlayer;
    [SerializeField] private bool isSpriteRightSide;
    private Vector3 _spawnPosition;
    protected Animator _animator;

    protected SpriteRenderer SpriteRenderer;

    protected bool hasTarget;
    public bool HasTarget => hasTarget;
    protected Transform _target;
    public Transform Target => _target;
    protected readonly Collider2D[] _colliders = new Collider2D[1];
    protected float lastAttackTime;

    private GameObject FloatingDamageObject;
    private Transform FloatingPos;
    [SerializeField] private bool isFly;
    
    private static readonly int RunAnimParam = Animator.StringToHash("run");
    private static readonly int Hit = Animator.StringToHash("hit");
    private static readonly int DieAnimParam = Animator.StringToHash("die");
    private static readonly int AttackAnimParam = Animator.StringToHash("attack");
    private static readonly int DistanceAnimParam = Animator.StringToHash("distance");

    protected override void Awake()
    {
        base.Awake();
        UpdateState.Add(CreatureState.Attack, OnAttack);
        UpdateState.Add(CreatureState.Run, OnRun);
        _animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        FloatingPos = transform.GetChild(0);
    }

    protected virtual void Start()
    {
        hasTarget = false;
        _target = null;
        _spawnPosition = transform.position;
        lastAttackTime = 0;
    }

    protected virtual void FixedUpdate() {
        
        if(IsDead || !Managers.Network.IsHost) return;
        if (!hasTarget)
        {
            var size = Physics2D.OverlapCircleNonAlloc(gameObject.transform.position, detectRadius, _colliders, chaseTargetLayerMask);
            if (size > 0)
            {
                _target = _colliders[0].transform;
                hasTarget = true;
            }
        }

        if (hasTarget)
        {
            if (_target == null)
            {
                hasTarget = false;
                return;
            }
            
            var distance = GetDistance(Target.transform.position);
            if (distance > detectRadius)
            {
                _target = null;
                hasTarget = false;
                return;
            }
            State = distance > minDisFromPlayer ? CreatureState.Run : CreatureState.Idle;
            _animator.SetFloat(DistanceAnimParam, distance);
        }
        // 만일 타겟을 가지고 있지 않다면
        else
        {
            var distance = GetDistance(_spawnPosition);
            State = distance > 1.0f ? CreatureState.Run : CreatureState.Idle;
        }

        if (State == CreatureState.Run)
        {
            var position = transform.position;
            Vector2 targetPosition = hasTarget ? Target.transform.position : _spawnPosition;
            DoFlip(targetPosition.x < position.x);
            var nextPosition = Vector2.MoveTowards(position, targetPosition, speed * Time.deltaTime);
            if (isFly || Managers.Map.CheckCanGo(nextPosition)) transform.position = nextPosition;
        }

        var packet = new S_Move
        {
            ObjectId = Id,
            PosInfo = new PositionInfo()
            {
                Dir = (int)transform.localScale.x,
                PosX = transform.position.x,
                PosY = transform.position.y,
                State = State
            }
        };
        Managers.Network.Server.Room.Broadcast(Managers.Network.LocalPlayer.Id, packet);
    }

    protected override void OnIdle(CreatureState state)
    {
        _animator.SetBool(RunAnimParam,false);
        if (!hasTarget || !(Time.timeSinceLevelLoad - lastAttackTime >= attackCooldown)) return;
        lastAttackTime = Time.timeSinceLevelLoad;
        State = CreatureState.Attack;
    }

    public virtual void OnRun(CreatureState state)
    {
        _animator.SetBool(RunAnimParam, true);
    }

    public void OnAttack(CreatureState state)
    {
        _animator.SetTrigger(AttackAnimParam);
    }

    public override void OnHit(CreatureState state)
    {
        _animator.SetTrigger(Hit);
    }

    public override void OnDamage(float damage)
    {
        base.OnDamage(damage);
        FloatingDamageObject = Managers.Resource.Instantiate("Objects/NonCharacter/FloatingDamageText");
        FloatingDamageObject.transform.position = FloatingPos.position;
        FloatingDamageObject.GetComponent<FloatingDamage>().SetText(damage);
    }

    protected override void OnDie(CreatureState state)
    {
        _animator.ResetTrigger(Hit);
        _animator.SetTrigger(DieAnimParam);
    }

    private void OnEndDieAnim()
    {
        if (!Managers.Network.IsHost) return;
        Managers.Network.Server.Room.SpawnLootingItems(4102,5,transform.position,2.0f, 1.0f);
        {
            S_DeSpawn despawn = new S_DeSpawn();
            despawn.ObjectIds.Add(Id);
            Managers.Network.Server.Room.Broadcast(despawn);
        }
    }

    private void FinishAttackAnim()
    {
        State = CreatureState.Idle;
    }

    private void FinishHitAnim()
    {
        if (hasTarget && GetDistance(Target.position) > minDisFromPlayer) State = CreatureState.Run;
        else State = CreatureState.Idle;
    }
    protected float GetDistance(Vector3 targetPosition)
    {
        return (targetPosition - transform.position).magnitude;
    }

    protected virtual void DoFlip(bool value)
    {
        if(GetComponent<SpriteRenderer>() == null) return;
        if(isSpriteRightSide)SpriteRenderer.flipX = value;
        else SpriteRenderer.flipX = !value;
    }
}
