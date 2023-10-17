using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class WeaponPivotController : NetworkObject
{
    [Tooltip("회전 반경")]
    [SerializeField] private float disFromBody;
    [Tooltip("회전 중심점")]
    [SerializeField] private Transform pivot;
    private Player _player;

    private Animator _animator;
    private Weapon _weapon;
    private static readonly int AttackAnimParam = Animator.StringToHash("attack");

    private void Awake()
    {
        _player = transform.parent.gameObject.GetComponent<Player>();
        _animator = transform.GetComponent<Animator>();
    }

    void Update()
    {
        if(_player != Managers.Network.LocalPlayer) return;
        if (_player.IsDead) return;

        // Get the Screen position of the mouse
        // Vector3 mouseOnScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseOnScreen = Camera.main.ScreenToWorldPoint(Managers.Input.UIActions.Point.ReadValue<Vector2>());
        
        Vector2 pos = pivot.position;
        Vector2 dir = (mouseOnScreen - pos).normalized;
        transform.position = dir * disFromBody + pos;
        transform.rotation = Quaternion.Euler(new Vector3(0.0f,0.0f,(Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg) + 180.0f));
    }

    public void Attack()
    {
        _animator.SetTrigger(AttackAnimParam);
    }

    public void OnEndAttack()
    {
        //;
    }
    
    
}
