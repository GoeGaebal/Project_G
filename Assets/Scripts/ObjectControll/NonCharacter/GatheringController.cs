using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Photon.Pun;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class GatheringController : DamageableEntity
{
    public int id = 1;
    public int guid = 1;
    public int lootingId = 1;
    public float flashDuration = 0.5f;
    public Color flashColor = Color.red;
    private Color _originalColor;
    private SpriteRenderer _sprite;
    private Animator _anim;
    public Image HPBar;
    public TextMeshProUGUI HPText;



    private void Init()
    {
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
            if (PhotonNetwork.IsMasterClient) Managers.Network.BroadCastObjectDestroy(guid);
        };
    }

    [PunRPC]
    public override void OnDamage(float damage)
    {
        if (PhotonNetwork.IsMasterClient) Managers.Network.BroadCastGatheringDamaged(guid, damage);
    }

    public void ApplyDamage(float damage)
    {
        if (isDead) return;
        HP -= damage;
        
        HPBar.fillAmount = HP / maxHP;
        HPText.text = $"{HP / maxHP:P2}";
        
        if (HP<=0 && !isDead)  Die();
        else StartCoroutine(nameof(CoFlashWhite));
        
    }

    public void ApplyDie()
    {
        // Managers.Object.SpawnLootingItems(lootingId,5,transform.position, 2.0f, 1.0f);
        
        Managers.Network.RequestSpawnLootingItems(lootingId, 5, transform.position, 2.0f, 1.0f);
        // TODO: 바로 삭제가 아니라 ObjectDict에서도 제외가 되어야 한다. 그래야 데이터가 연동 되기 때문이다. (ObjectDict이 좌표 혹은 아예 고유한 id를 key로 받게끔 조정해야 된다. )
        Managers.Object.LocalObjectsDict.Remove(guid);
        Managers.Object.ObjectInfos.Remove(guid);
        StartCoroutine(nameof(DelayDeath));
        
    }

    private IEnumerator CoFlashWhite()
    {
        _anim.Play("ScrapDamage");
        yield return new WaitForSeconds(flashDuration);
        _anim.Play("Idle");
    }
    
    private IEnumerator DelayDeath()
    {
        _anim.Play("ScrapDeath");
        yield return new WaitForSeconds(flashDuration);
        Managers.Resource.Destroy(gameObject);
    }
}
