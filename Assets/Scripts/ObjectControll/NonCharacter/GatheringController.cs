using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Google.Protobuf.Protocol;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class GatheringController : DamageableEntity
{
    public int id = 1;
    public int guid = 1;
    public int lootingId = 5101;
    public float flashDuration = 1.0f;
    public Color flashColor = Color.red;
    private Color _originalColor;
    private SpriteRenderer _sprite;
    private Animator _anim;
    public Image HPBar;
    public TextMeshProUGUI HPText;



    private void Init()
    {
        if (HPBar == null) HPBar = Util.FindChild(gameObject,"HP", true).GetComponent<Image>();
        if (HPText == null) HPText = Util.FindChild(gameObject,"HPText", true).GetComponent<TextMeshProUGUI>();
        maxHP =  Managers.Data.GatheringDict[id].maxHp;
        HP = maxHP;
        lootingId = Managers.Data.GatheringDict[id].lootingId;
        HPBar.fillAmount = HP / maxHP;
        HPText.text = $"{HP / maxHP:P2}";
    }
    private void Start()
    {
        Init();
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _originalColor = _sprite.color;
        base.dieAction += () =>
        {
            StartDelayDeath();
        };
    }
    
    public override void OnDamage(float damage)
    {
        base.OnDamage(damage);
        if(!isDead) _anim.Play("ScrapDamage");
    }

    public override void UpdateHP(float health, bool dead)
    {
        HPBar.fillAmount = HP / maxHP;
        HPText.text = $"{HP / maxHP:P2}";
    }

    private IEnumerator DelayDeath()
    {
        _anim.Play("ScrapDeath");
        yield return new WaitForSeconds(flashDuration);
        if (Managers.Network.isHost)
        {
            Managers.Network.Server.Room.SpawnLootingItems(lootingId,5,transform.position, 1.0f, 2.0f);
            S_Despawn packet = new S_Despawn();
            packet.ObjectIds.Add(Id);
            Managers.Network.Server.Room.Broadcast(packet);
        }
    }
    
    public void StartDelayDeath()
    {
        StartCoroutine(nameof(DelayDeath));
    }
}
