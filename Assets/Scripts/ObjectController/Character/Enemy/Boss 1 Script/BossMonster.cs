using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;



public class BossMonster : BasicMonster
{   
    public static Action FinishSpellAction;
    [SerializeField] private Vector2 BossroomLeftDownWorldPos;
    [SerializeField] private Vector2 BossroomRightUpWorldPos;
    private readonly List<ICastingSpell> _spells = new();
    public List<Transform> Thunders { get; } = new();

    private bool _canCastingState = true;
    
     protected override void Start() {
        base.Start();

        FinishSpellAction = FinishSpell; 
        
        _spells.Add(new NEWSSpell(this));
        _spells.Add(new TeleportTargetSpell(this));

        for(int i = 0;i< 4;i++)
        {
            Debug.Log("BossThunder"+i.ToString());
            Thunders.Add(transform.Find(string.Format("BossThunder{0}",i)));
        }
    }

    [SerializeField] float cooltimeCasting;
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

    protected override void OnDie()
    {
        base.OnDie();
        if (!Managers.Network.IsHost) return;
        Managers.Network.Server.Room.SpawnLootingItems(5101, 10, transform.position, 2.0f, 1.0f);
        Managers.Network.Server.Room.SpawnLootingItems(5001, 5, transform.position, 2.0f, 1.0f);
    }
    
    private void OnFinishDieAnim()
    {
        if (!Managers.Network.IsHost) return;
        Managers.Network.Server.Room.SpawnLootingItems(5001,5,transform.position,2.0f, 1.0f);
        {
            S_DeSpawn despawn = new S_DeSpawn();
            despawn.ObjectIds.Add(Id);
            Managers.Network.Server.Room.Broadcast(despawn);
        }
    }

    class NEWSSpell: ICastingSpell
    {
        BossMonster _bossMonster;
        public NEWSSpell(BossMonster bm)
        {
            _bossMonster = bm;
        }
        public void Spell()
        {
            Debug.Log("do spell");
            List<Transform> tfs = _bossMonster.Thunders;
            foreach(Transform tf in tfs)
            {
                tf.gameObject.SetActive(true);
            }
            tfs[0].localPosition = new Vector2(0f,0.5f);
            tfs[1].localPosition = new Vector2(0.5f,0f);
            tfs[2].localPosition = new Vector2(0f,-0.5f);
            tfs[3].localPosition = new Vector2(-0.5f,0f);

            foreach(Transform tf in tfs)
            {
                tf.SetParent(null);
            }
            
        }
    }

    class TeleportTargetSpell:ICastingSpell
    {
        BossMonster _bossMonster;
        public TeleportTargetSpell(BossMonster bm)
        {
            _bossMonster = bm;
        }
        public void Spell()
        {
            if (_bossMonster.Target == null)
            {
                return;
            }
            _bossMonster.Target.transform.position = (Vector2)_bossMonster.transform.position + Vector2.right * 0.5f;
        }
    }

    protected override void OnRun()
    {
        base.OnRun();
        var position = transform.position;
        float xpos = Mathf.Clamp(position.x, BossroomLeftDownWorldPos.x, BossroomRightUpWorldPos.x);
        float ypos = Mathf.Clamp(position.y, BossroomLeftDownWorldPos.y, BossroomRightUpWorldPos.y);

        position = new Vector3(xpos,ypos, position.z);
        transform.position = position;
    }
}
