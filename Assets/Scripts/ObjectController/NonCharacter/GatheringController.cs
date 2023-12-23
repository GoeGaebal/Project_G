using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Google.Protobuf.Protocol;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GatheringController : CreatureController
{
    public int id = 1;
    public int guid = 1;
    public int lootingId = 4101;
    public float flashDuration = 1.0f;
    public Color flashColor = Color.red;
    private Color _originalColor;
    private SpriteRenderer _sprite;
    private Animator _anim;
    public Image HPBar;
    public TextMeshProUGUI HPText;
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Die = Animator.StringToHash("Die");
    private bool _isHitting = false; 


    protected override void Awake()
    {
        base.Awake();
        if (HPBar == null) HPBar = Util.FindChild(gameObject,"HP", true).GetComponent<Image>();
        if (HPText == null) HPText = Util.FindChild(gameObject,"HPText", true).GetComponent<TextMeshProUGUI>();
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
    }

    protected override void OnIdle(CreatureState state)
    {
        _anim.ResetTrigger(Hit);
        _anim.ResetTrigger(Die);
    }

    private void Init()
    {
        StatInfo.MaxHp =  Managers.Data.GatheringDict[id].maxHp;
        UpdateHp(maxHP,IsDead);
        lootingId = Managers.Data.GatheringDict[id].lootingId;
    }
    private void Start()
    {
        Init();
        _originalColor = _sprite.color;
    }

    public override void OnHit(CreatureState state)
    {
        if (IsDead || _isHitting) return;
        _anim.SetTrigger(Hit);
        _isHitting = true;
    }

    protected override void OnDie(CreatureState state)
    {
        _anim.SetTrigger(Die);
        base.OnDie(state);
    }

    public override void UpdateHp(float health, bool dead)
    {
        base.UpdateHp(health,dead);
        HPBar.fillAmount = HP / maxHP;
        HPText.text = $"{HP / maxHP:P2}";
    }

    private void FinishDieAnim()
    {
        if (!Managers.Network.IsHost) return;
        Managers.Network.Server.Room.SpawnLootingItems(lootingId,5,transform.position, 1.0f, 2.0f);
        S_DeSpawn packet = new S_DeSpawn();
        packet.ObjectIds.Add(Id);
        Managers.Network.Server.Room.Broadcast(packet);
    }

    private void FinishHitAnim()
    {
        State = CreatureState.Idle;
        _isHitting = false;
    }
}
