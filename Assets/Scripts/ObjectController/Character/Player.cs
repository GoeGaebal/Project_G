using System;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.InputSystem;
using Server;

public class Player : CreatureController
{
    [SerializeField] private float moveSpeed = 5.0f;

    public float attackDamage = 10f;

    public float[] artifactDamage = new float[] { 0f, 0f, 0f };//유물 대미지 (곱연산)
    public float[] equipDamage = new float[] { 0f, 0f, 0f, 0f, 0f };//장비 대미지 (합연산)

    public float[] artifactHP = new float[] { 0f, 0f, 0f };//유물 체력 (곱연산)
    public float[] equipHP = new float[] { 0f, 0f, 0f, 0f, 0f };//장비 체력 (합연산)

    public float realDamage = 0f;//장비, 유물 스탯 연산 이후 공격력

    private PlayerCameraController playerCameraController;
    private WeaponPivotController _wpc;
    private Vector2 _moveInput;

    private Rigidbody2D _rb;

    [SerializeField] private Animator bodyAnimator;
    [SerializeField]private Animator legAnimator;

    private CreatureState _state;
    private Coroutine resetAttackCountCoroutine;
    private bool attackInputBuffer = false;
    private Vector2 runInputBuffer = Vector2.zero;

    public ClientSession Session => Managers.Network.Server.Room.PlayersSessions[Id];

    IInteractable interactable;
    string interactableName;
    private static readonly int DieAnimParam = Animator.StringToHash("die");
    private static readonly int WalkAnimParam = Animator.StringToHash("walk");
    private static readonly int HitAnimParam = Animator.StringToHash("hit");

    [SerializeField] private GameObject Left_Arm;
    [SerializeField] private GameObject Right_Arm;
    
    private void Awake()
    {
        ObjectType = GameObjectType.Player;
        _wpc = transform.GetComponentInChildren<WeaponPivotController>();
        _rb = GetComponent<Rigidbody2D>();
        PosInfo.Dir = 1;
    }

    protected override void OnEnable() {
        base.OnEnable();

        State = CreatureState.Idle;
        //카메라 이동 제한
        if (Managers.Network.LocalPlayer != this) return;
        
        if (Camera.main != null) playerCameraController = Camera.main.GetComponent<PlayerCameraController>();
        playerCameraController.SetPosition(transform.position);
        if(playerCameraController.enabled)
            playerCameraController.enabled = false;
    }

    protected override void OnIdle()
    {
        bodyAnimator.SetBool(WalkAnimParam,false);
        legAnimator.SetBool(WalkAnimParam,false);
    }

    protected override void OnRun()
    {
        bodyAnimator.SetBool(WalkAnimParam,true);
        legAnimator.SetBool(WalkAnimParam,true);
    }

    protected override void OnDie()
    {
        bodyAnimator.SetTrigger(DieAnimParam);
        legAnimator.SetTrigger(DieAnimParam);

        if (Managers.Scene.CurrentScene is not GameScene) return;
        if(GameScene.PlayerLifeCnt > 0) GameScene.PlayerLifeCnt --;
    }

    protected override void OnAttack()
    {
        _wpc.Attack(realDamage);
    }

    protected override void OnHit()
    {
        bodyAnimator.SetTrigger(HitAnimParam);
        // StartCoroutine(HitStateCoroutine());
    }

    void Start()
    {
        playerCameraController = Camera.main.GetComponent<PlayerCameraController>();
        playerCameraController.enabled = false;
    }

    private void Update()
    {
        realDamage = (attackDamage + equipDamage[0] + equipDamage[1] + equipDamage[2] + equipDamage[3] + equipDamage[4]) * (1 + artifactDamage[0]) * (1 + artifactDamage[1]) * (1 + artifactDamage[2]);
    }

    public void BindingAction()
    {
        Managers.Input.PlayerActions.Move.AddEvent(OnMoveInput);
        Managers.Input.PlayerActions.Attack.AddEvent(OnAttackInput);
        Managers.Input.PlayerActions.Interact.AddEvent(OnInteract);
    }

    private void FixedUpdate()
    {
        if(Managers.Network.LocalPlayer != this) return;
        if(IsDead) return;

        // Vector3 mousePos = Mouse.current.position.value;
        Vector3 mousePos = Managers.Input.UIActions.Point.ReadValue<Vector2>();
        mousePos.z = Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        
        Flip(worldPos.x >=  transform.position.x);

        if (State == CreatureState.Run || State == CreatureState.Attack)
        {
            Vector3 dest = _rb.position + _moveInput * moveSpeed * Time.fixedDeltaTime;
            if (Managers.Map.CheckCanGo(dest)) _rb.MovePosition(dest);
        }
        // 매번 패킷을 보낸다.
        var position = transform.position;
        PosInfo.PosX = position.x;
        PosInfo.PosY = position.y;
        PosInfo.Dir = (int)transform.localScale.x;
        PosInfo.State = State;
        Info.PosInfo = PosInfo;

        var t = _wpc.transform;
        var p = t.localPosition;
        var packet = new C_PlayerMove
        {
            PosInfo = new PlayerPosInfo()
            {
                PosInfo = PosInfo,
                WPosX = p.x,
                WPosY = p.y,
                WRotZ = t.localEulerAngles.z
            }
        };
        Managers.Network.Client.Send(packet);
    }

    private void Flip(bool isFlip)
    {
        Left_Arm.SetActive(isFlip);
        Right_Arm.SetActive(!isFlip);
        Vector2 localSc = transform.localScale;
        localSc.x = isFlip ? -1 * Math.Abs(localSc.x) : localSc.x = Math.Abs(localSc.x);
        transform.localScale = localSc;
        Vector2 wpcLocalScale = _wpc.transform.localScale;
        _wpc.transform.localScale = 
            (isFlip) ? new Vector2(-Math.Abs(wpcLocalScale.x), -Math.Abs(wpcLocalScale.y)) : new Vector2(Math.Abs(wpcLocalScale.x), Math.Abs(wpcLocalScale.y));
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        if(IsDead || Managers.Network.LocalPlayer != this) return;

        _moveInput = context.ReadValue<Vector2>();
        if(_moveInput == null) return;
        State = context.canceled ? CreatureState.Idle : CreatureState.Run;
    }   
    
    private void LateUpdate()
    {
        if(IsDead) return;

        if (Managers.Network.LocalPlayer == this)
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
    
    public override void OnDamage(float damage)
    {
        if (!Managers.Network.IsHost) return;
        
        if(IsDead) return;
        base.OnDamage(damage);
        if(State == CreatureState.Idle || State == CreatureState.Run) 
            State = CreatureState.Hit;
        
    }

    // private  IEnumerator HitStateCoroutine()
    // {
    //     bodyAnimator.SetTrigger("hit");
    //     yield return new WaitForSeconds(0.5f);
    //     if (!runInputBuffer.Equals(Vector2.zero))
    //     {
    //         State = CreatureState.Run;
    //         moveInput = runInputBuffer;
    //         runInputBuffer = Vector2.zero;
    //     }
    //     else State = CreatureState.Idle;
    // }

    ///
    ///<summary>
    ///공격 입력이 들어왔을 때 가장 먼저 호출되는 함수
    ///</summary>
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if(IsDead) return;
        if(!context.started) return;
        if(Managers.Network.LocalPlayer != this) return;
        if(State != CreatureState.Idle && State != CreatureState.Attack && State != CreatureState.Run) return;
        
        // //이동중간에 액션 들어올 경우를 대비해서, 공격 시작 시 위치 고정
        // rb.velocity = Vector2.zero;

        if( State == CreatureState.Attack && bodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
            attackInputBuffer = true;

        // else if (State == EnumPlayerStates.Run)
        //     animator.SetBool("run",false);
        State = CreatureState.Attack;
    }
    
    public void FinishAttackState()
    {
        if (this == Managers.Network.LocalPlayer)
            State = Managers.Input.PlayerActions.Move.IsPressed() ? CreatureState.Run : CreatureState.Idle;
        else State = CreatureState.Idle;
    }

    public void FinishDieAnimClip()
    {
        gameObject.SetActive(false);

        if (Managers.Network.LocalPlayer == this)
        {
            if (Camera.main != null) playerCameraController = Camera.main.GetComponent<PlayerCameraController>();
            if (!playerCameraController.enabled)
                playerCameraController.enabled = true;
            playerCameraController.SetPosition(transform.position);
        }
    }

    private void OnDestroy()
    {
        Managers.Input.PlayerActions.Move.RemoveEvent(OnMoveInput);
        Managers.Input.PlayerActions.Attack.RemoveEvent(OnAttackInput);
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

    public override void SyncPos()
    {
        base.SyncPos();
        State = PosInfo.State;
        transform.localScale = new Vector3(PosInfo.Dir,1, 1);
        _wpc.transform.localScale = new Vector3(PosInfo.Dir, PosInfo.Dir, 1);
        Left_Arm.SetActive(PosInfo.Dir == -1);
        Right_Arm.SetActive(PosInfo.Dir != -1);
    }

    public void SyncWPos(float WposX, float WposY, float WrotZ)
    {
        var t = _wpc.transform;
        t.localPosition = new Vector3(WposX, WposY);
        t.localEulerAngles = new Vector3(0, 0, WrotZ);
    }
}
