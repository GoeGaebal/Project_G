using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEditor;

public class GatheringController : DamageableEntity
{
    public int id = 1;
    public int lootingId = 1;
    public float flashDuration = 0.5f;
    public Color flashColor = Color.red;
    private Color _originalColor;
    private SpriteRenderer _sprite;
    private Animator _anim;

    private void Init()
    {
        maxHP =  Managers.Data.GatheringDict[id].maxHp;
        HP = maxHP;
        lootingId = Managers.Data.GatheringDict[id].lootingId;
    }
    private void Start()
    {
        Init();
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _originalColor = _sprite.color;
        base.dieAction += () =>
        {
            Managers.Object.SpawnLootings(lootingId,5,transform.position, 2.0f, 1.0f);
            // TODO: 바로 삭제가 아니라 ObjectDict에서도 제외가 되어야 한다. 그래야 데이터가 연동 되기 때문이다. (ObjectDict이 좌표 혹은 아예 고유한 id를 key로 받게끔 조정해야 된다. )
            Managers.Resource.Destroy(gameObject);
        };
    }

    public override void OnDamage(float damage)
    {
        base.OnDamage(damage);
        StartCoroutine(nameof(CoFlashWhite));
    }

    private IEnumerator CoFlashWhite()
    {
        // _sprite.color = flashColor;
        _anim.Play("rock_damaged");
        yield return new WaitForSeconds(flashDuration);
        _anim.Play("rock_idle");
        // _sprite.color = _originalColor;
    }
    // TODO : 피격하며 죽을 시 아이템 뿌리기
}
