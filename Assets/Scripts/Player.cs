using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EnumPlayerStates
{
    Idle, Attack1, Attack2, Attack3, Run, Hit
}

public class Player : DamageableEntity
{
    [SerializeField]private float moveSpeed = 5.0f;
    private InputAction inputAction; 
    private PlayerInput playerInput; 
    private Vector2 moveInput;
    private Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    private EnumPlayerStates _state;

    public EnumPlayerStates State
    {
        get{
            return _state;
        }
        protected set{
                _state = value;
                UpdateState();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        inputAction = playerInput.actions["Move"];

        dieAction += () => {
            animator.SetTrigger("die");
        };

        Debug.Log(inputAction == null);

    }
    


    private void FixedUpdate()
    {
        if(isDead) return;

        

        if(State == EnumPlayerStates.Run)
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {

        if (photonView.IsMine)
        {
             if(isDead) return;

            moveInput = context.ReadValue<Vector2>();
            if(moveInput == null) return;

            if(moveInput.x >0) spriteRenderer.flipX = false;
            else if (moveInput.x <0) spriteRenderer.flipX = true;

            if(State == EnumPlayerStates.Idle && inputAction.IsPressed())
                State = EnumPlayerStates.Run;
            else if(State == EnumPlayerStates.Run && !inputAction.IsPressed())
            State = EnumPlayerStates.Idle;
            
            
        }
    }   

    private void LateUpdate()
    {
        if(isDead) return;

        if (photonView.IsMine)
        {
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y,-10);
        }
    }

    private void UpdateState()
    {
        switch (State)
        {
            case EnumPlayerStates.Idle:
                animator.SetBool("run",false);
                break;
            case EnumPlayerStates.Run:
                animator.SetBool("run",true);
                break;
            case EnumPlayerStates.Hit:
                StartCoroutine("HitCoroutine");
                break;
            default:
                break;
        }
    }

    public override void OnDamage(float damage)
    {
        if(isDead) return;
        Debug.Log("damage");
        base.OnDamage(damage);
        if(State == EnumPlayerStates.Idle || State == EnumPlayerStates.Run) 
            State = EnumPlayerStates.Hit;
        
    }

    private IEnumerator HitCoroutine()
    {
        animator.SetTrigger("hit");
        yield return new WaitForSeconds(1.5f);
        State = EnumPlayerStates.Idle;
    }
    public void asd(InputAction.CallbackContext context)
    {
        if (context.started)
        Debug.Log("click");
    }
}
