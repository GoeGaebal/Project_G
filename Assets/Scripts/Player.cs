using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EnumPlayerStates
{
    Idle, Attack, Run, Hit
}

public class Player : DamageableEntity
{
    [SerializeField]private float moveSpeed = 5.0f;
    [SerializeField] private float attackDelay = 1.5f; 

    private InputAction inputAction; 
    private PlayerInput playerInput; 
    private Vector2 moveInput;
    private Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    private EnumPlayerStates _state;
    private Coroutine resetAttackCountCoroutine;
   
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

            if(State == EnumPlayerStates.Idle && context.started)
                State = EnumPlayerStates.Run;
            else if(State == EnumPlayerStates.Run && context.canceled)
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

    ///
    ///<summary>
    ///공격 입력이 들어왔을 때 가장 먼저 호출되는 함수
    ///</summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(isDead) return;
        if(!context.started) return;
        if(State != EnumPlayerStates.Idle && State != EnumPlayerStates.Attack && State != EnumPlayerStates.Run) return;
      
    
        //이전에 돌아가고 있던 코루틴이 있으면 종료 시킴.
        if(resetAttackCountCoroutine != null)
            StopCoroutine(resetAttackCountCoroutine);
        resetAttackCountCoroutine = StartCoroutine(ResetAttackCountCoroutine());

        State = EnumPlayerStates.Attack;
        animator.SetBool("attack",true);

    }

    

    ///<summary>
    ///마지막 공격에서부터 얼마나 시간이 걸렸는지 체크한다.
    ///공격 할 때마다 호출 됨. 마지막 공격으로부터 attackDelay만큼의 시간이 지나면 attackCounter를 0으로 만듦
    ///</summary>
    private IEnumerator ResetAttackCountCoroutine()
    {
        float curTime = 0f;
        while(true)
        {
            curTime+=Time.deltaTime;
            if(curTime >= attackDelay)
            {
                break;
            }
            yield return null;
        }

        animator.SetInteger("attackCount",0);
    }

    public void FinishAttackState()
    {
        if(State != EnumPlayerStates.Attack) return;

        Debug.Log(animator.GetInteger("attackCount"));
        animator.SetInteger("attackCount",(animator.GetInteger("attackCount")+1)%3);

        //공격 애니메이션을 마치고 조금 더 매끄러운 이동을 위한 코드
        if(inputAction.IsPressed()) State = EnumPlayerStates.Run;
        else State = EnumPlayerStates.Idle;
        animator.SetBool("attack",false);
    }

}
