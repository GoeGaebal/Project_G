using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;



public partial class BossMonster : BasicMonster
{   
    public static Action FinishSpellAction;
    [SerializeField] private Vector2 BossroomLeftDownWorldPos;
    [SerializeField] private Vector2 BossroomRightUpWorldPos;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] float cooltimeCasting;
    private readonly List<ICastingSpell> _spells = new();
    public List<Transform> Thunders { get; } = new();

    private bool _canCastingState = true;
    
     protected override void Start() {
        base.Start();

        FinishSpellAction = FinishSpell; 
        
        _spells.Add(new ThunderSpell(this));
        _spells.Add(new TeleportTargetSpell(this));

        for(int i = 0;i< 4;i++)
        {
            Debug.Log("BossThunder"+i.ToString());
            Thunders.Add(transform.Find(string.Format("BossThunder{0}",i)));
        }
    }


    private static readonly int Casting = Animator.StringToHash("casting");

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if(Target==null) return;
        if(Target.GetComponent<CreatureController>().IsDead) return;
        if(IsDead) return;
        if(hasTarget == false || Target == null) return;

        if (!_canCastingState || (State is not (CreatureState.Run or CreatureState.Idle))) return;
        StartCoroutine(nameof(CooldownTimeCastingState));
        _animator.SetTrigger(Casting);
    }

    public void FinishSpell()
    {
        foreach(var thunder in Thunders)
        {
            thunder.SetParent(transform);
            thunder.gameObject.SetActive(false);
        }
    }

    public void DoSpell()
    {
        _spells[UnityEngine.Random.Range(0,_spells.Count)].Spell();
    }
    public void FinishCasthingState()
    {
        State = CreatureState.Idle;
    }

    private IEnumerator CooldownTimeCastingState()
    {
        _canCastingState = false;

        yield return new WaitForSeconds(cooltimeCasting);
        _canCastingState = true;
    }

    protected override void OnDie(CreatureState state)
    {
        base.OnDie(state);
        Instantiate(portalPrefab,transform.position, transform.rotation);

        if (!Managers.Network.IsHost) return;
        Managers.Network.Server.Room.SpawnLootingItems(4101, 25, transform.position, 2.0f, 1.0f);
        Managers.Network.Server.Room.SpawnLootingItems(4102, 10, transform.position, 2.0f, 1.0f);
        Managers.Network.Server.Room.SpawnLootingItems(4103, 5, transform.position, 2.0f, 1.0f);
    }
    
    private void OnFinishDieAnim()
    {
        if (!Managers.Network.IsHost) return;
        Managers.Network.Server.Room.SpawnLootingItems(4101,5,transform.position,2.0f, 1.0f);
        {
            S_DeSpawn despawn = new S_DeSpawn();
            despawn.ObjectIds.Add(Id);
            Managers.Network.Server.Room.Broadcast(despawn);
        }
    }

    public override void OnRun(CreatureState state)
    {
        base.OnRun(state);
        var position = transform.position;
        float xpos = Mathf.Clamp(position.x, BossroomLeftDownWorldPos.x, BossroomRightUpWorldPos.x);
        float ypos = Mathf.Clamp(position.y, BossroomLeftDownWorldPos.y, BossroomRightUpWorldPos.y);

        position = new Vector3(xpos,ypos, position.z);
        transform.position = position;
    }
}
