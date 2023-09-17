using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public enum EnumPlayerStates
{
    Idle, Attack, Run, Hit
}

public class Player : DamageableEntity
{
    [SerializeField] private float moveSpeed = 5.0f;
    
    private PlayerCameraController playerCameraController;
    private Vector2 moveInput;

    private Rigidbody2D rb;

    protected Animator animator;

    private EnumPlayerStates _state;
    private Coroutine resetAttackCountCoroutine;
    private bool attackInputBuffer = false;
    private Vector2 runInputBuffer = Vector2.zero;

    private PhotonView[] _photonViews;
    public PhotonView[] PhotonViews {
        get
        {
            _photonViews ??= gameObject.GetPhotonViewsInChildren();
            return _photonViews;
        }
    }
    IInteractable interactable;
    string interactableName;

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
    
    protected override void OnEnable() {
        base.OnEnable();
        State = EnumPlayerStates.Idle;
        //카메라 이동 제한
        if(photonView.IsMine)
        {
            playerCameraController ??= Camera.main.gameObject.GetComponent<PlayerCameraController>();
            playerCameraController.SetPosition(transform.position);
            if(playerCameraController.enabled)
                playerCameraController.enabled = false;
        }
        
        
        
    }

    private void UpdateState()
    {
        if(isDead || animator == null) return;
        switch (State)
        {
            
            case EnumPlayerStates.Idle:
                animator.SetBool("run",false);
                break;
            case EnumPlayerStates.Run:
                animator.SetBool("run",true);
                break;
            case EnumPlayerStates.Hit:
                //hit를 코루틴이 애니메이션 이벤트로 한 번 해보자
                StartCoroutine(HitStateCoroutine());
                break;
            case EnumPlayerStates.Attack:
                animator.SetTrigger("attack");
                break;
            default:
                break;
            
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
        playerCameraController = GameObject.Find("Main Camera").GetComponent<PlayerCameraController>();
        playerCameraController.enabled = false;
        BindingAction();

        dieAction += () => {
            animator.SetTrigger("die");

            if(Managers.Scene.CurrentScene is GameScene)
            {
                if(GameScene.PlayerLifeCnt > 0) GameScene.PlayerLifeCnt --;
            }
        };
    }

    public void BindingAction()
    {
        Managers.Input.PlayerActions.Move.AddEvent(OnMove);
        Managers.Input.PlayerActions.Attack.AddEvent(OnAttack);
        Managers.Input.PlayerActions.Interact.AddEvent(OnInteract);
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine) return;
        if(isDead) return;

        // Vector3 mousePos = Mouse.current.position.value;
        Vector3 mousePos = Managers.Input.UIActions.Point.ReadValue<Vector2>();
        mousePos.z = Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if(worldPos.x >=  transform.position.x)
        {
            Vector2 localSc = transform.localScale;
            localSc.x = -1 * Math.Abs(localSc.x);
            transform.localScale = localSc;
        }
        else
        {
            Vector2 localSc = transform.localScale;
            localSc.x = Math.Abs(localSc.x);
            transform.localScale = localSc;
        }


        if (State == EnumPlayerStates.Run || State == EnumPlayerStates.Attack)
        {
            Vector3 dest = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
            if (Managers.Map.CheckCanGo(dest))
            {
                rb.MovePosition(dest);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (photonView.IsMine)
        {
             if(isDead) return;
             moveInput = context.ReadValue<Vector2>();
             
             if(moveInput == null) return;
             
            switch(State)
            {
                case EnumPlayerStates.Idle:
                 if((context.performed || context.started))
                    State = EnumPlayerStates.Run;
                    break;
                case EnumPlayerStates.Run:
                    if (context.canceled)
                        State = EnumPlayerStates.Idle;
                    break;
                case EnumPlayerStates.Attack:
                    if(context.started)
                    {
                        //공격 중에 이동 키 눌리면 선입력 버퍼에 저장하고 리턴
                        runInputBuffer  = moveInput;
                        return;
                    }
                    break;
                case EnumPlayerStates.Hit:
                    if(context.started)
                    {
                        //피격 중에 이동 키 눌리면 선입력 버퍼에 저장하고 리턴
                        runInputBuffer  = moveInput;
                        return;
                    }
                    break;
                default: 
                    break;
            }
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
    
    [PunRPC]
    public override void OnDamage(float damage)
    {
        if(isDead) return;
        base.OnDamage(damage);
        if(State == EnumPlayerStates.Idle || State == EnumPlayerStates.Run) 
            State = EnumPlayerStates.Hit;
        
    }

    private  IEnumerator HitStateCoroutine()
    {
        animator.SetTrigger("hit");
        yield return new WaitForSeconds(0.5f);
        if (!runInputBuffer.Equals(Vector2.zero))
        {
            State = EnumPlayerStates.Run;
            moveInput = runInputBuffer;
            runInputBuffer = Vector2.zero;
        }
        else State = EnumPlayerStates.Idle;
    }

    ///
    ///<summary>
    ///공격 입력이 들어왔을 때 가장 먼저 호출되는 함수
    ///</summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(isDead) return;
        if(!context.started) return;
        if(!photonView.IsMine) return;
        if(State != EnumPlayerStates.Idle && State != EnumPlayerStates.Attack && State != EnumPlayerStates.Run) return;
        
        //이동중간에 액션 들어올 경우를 대비해서, 공격 시작 시 위치 고정
        rb.velocity = Vector2.zero;

        if( State == EnumPlayerStates.Attack && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
            attackInputBuffer = true;

        // else if (State == EnumPlayerStates.Run)
        //     animator.SetBool("run",false);
        State = EnumPlayerStates.Attack;
    }
    
    public void FinishAttackState()
    {
        if(attackInputBuffer) 
        {
            attackInputBuffer = false;
        }
        else if (Managers.Input.PlayerActions.Move.IsPressed())
        {
            State = EnumPlayerStates.Run;
            moveInput = runInputBuffer;
            runInputBuffer = Vector2.zero;
        }
        else 
        {
            State = EnumPlayerStates.Idle;
        }
    }

    public void FinishDieAnimClip()
    {

        gameObject.SetActive(false);

        if (photonView.IsMine)
        {
            if (!playerCameraController.enabled)
                playerCameraController.enabled = true;
            playerCameraController.SetPosition(transform.position);
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            Managers.Input.PlayerActions.Move.RemoveEvent(OnMove);
            Managers.Input.PlayerActions.Attack.RemoveEvent(OnAttack);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(interactable == null)
        {
            interactable = collision.GetComponent<IInteractable>();
            interactableName = collision.gameObject.name;
        }
        else
        {
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interactableName == collision.gameObject.name)
        {
            interactable = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(interactable == null)
        {
            interactable = collision.GetComponent<IInteractable>();
            interactableName = collision.gameObject.name;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (interactable != null)
        {
            interactable.Interact();
        }
    }
}
