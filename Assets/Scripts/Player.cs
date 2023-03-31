using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EnumPlayerStates
{
    Idle, Attack1, Attack2, Attack3, Run
}

public class Player : DamageableEntity
{
    [SerializeField]private float moveSpeed = 5.0f;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    private 


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnMove(InputValue value)
    {
        if (photonView.IsMine)
        {
            moveInput = value.Get<Vector2>();
        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y,-10);
        }
    }
}
