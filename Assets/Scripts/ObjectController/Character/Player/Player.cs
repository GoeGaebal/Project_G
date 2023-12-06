using System;
using System.Collections;
using Google.Protobuf.Protocol;
using ObjectController.Character.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Server;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Player : CreatureController, IAttackable, IMoveable
{
    [SerializeField] private float moveSpeed = 5.0f;

    public float attackDamage = 100f;

    public float[] artifactDamage = new float[] { 0f, 0f, 0f };//유물 대미지 (곱연산)
    public float[] equipDamage = new float[] { 0f, 0f, 0f, 0f, 0f };//장비 대미지 (합연산)

    public float[] artifactHP = new float[] { 0f, 0f, 0f };//유물 체력 (곱연산)
    public float[] equipHP = new float[] { 0f, 0f, 0f, 0f, 0f };//장비 체력 (합연산)

    public float realDamage = 0f;//장비, 유물 스탯 연산 이후 공격력

    private PlayerCameraController playerCameraController;
    private WeaponPivotController _wpc;
    private Vector2 _moveInput;

    private Rigidbody2D _rb;


    private CreatureState _state;
    private Coroutine resetAttackCountCoroutine;
    private bool _attackInputBuffer = false;
    private Vector2 _runInputBuffer = Vector2.zero;

    private AnimationEvent OnFinishDieAnim;

    public ClientSession Session => Managers.Network.Server.Room.PlayersSessions[Id];

    IInteractable interactable;
    string interactableName;

    [SerializeField] private PlayerBody _playerBody;
    [SerializeField] private PlayerLeg _playerLeg;
    [SerializeField] private GameObject Left_Arm;
    [SerializeField] private GameObject Right_Arm;
    [SerializeField] private Transform _weapon_pivot;
    
    public Image HPBar;
    public TextMeshProUGUI HPText;

    public string Name
    {
        get => _name.text;
        set
        {
            Info.Name = value;
            _name.SetText(value);
        }
    }
    private TextMeshProUGUI _name;
    
    
    #region UnityMessages
    protected override void Awake()
    {
        base.Awake();
        UpdateState.Add(CreatureState.Attack, OnAttack);
        UpdateState.Add(CreatureState.Run, OnRun);
        
        if (HPBar == null) HPBar = Util.FindChild(gameObject,"HP", true).GetComponent<Image>();
        var texts = GetComponentsInChildren<TextMeshProUGUI>();
        _name = texts[1];
        HPText = texts[0];
        ObjectType = GameObjectType.Player;
        _wpc = GetComponentInChildren<WeaponPivotController>();
        _rb = GetComponent<Rigidbody2D>();
        _playerBody = GetComponentInChildren<PlayerBody>();
        _playerLeg = GetComponentInChildren<PlayerLeg>();
        _playerBody.Init();
        _playerLeg.Init();

        PosInfo.Dir = 1;

        _playerBody.OnFinishDie = null;
        _playerBody.OnFinishDie = () =>
        {
            gameObject.SetActive(false);

            if (Managers.Network.LocalPlayer == this)
            {
                if (Camera.main != null) playerCameraController = Camera.main.GetComponent<PlayerCameraController>();
                if (!playerCameraController.enabled)
                    playerCameraController.enabled = true;
                playerCameraController.SetPosition(transform.position);
            }
        };
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
    
    private void LateUpdate()
    {
        if(IsDead) return;

        if (Managers.Network.LocalPlayer == this)
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
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
    #endregion

    #region State
    protected override void OnIdle(CreatureState state)
    {
        _playerBody.Walk(false);
        _playerLeg.Walk(false);
    }

    public virtual void OnRun(CreatureState state)
    {
        _playerBody.Walk(true);
        _playerLeg.Walk(true);
    }

    protected override void OnDie(CreatureState state)
    {
        base.OnDie(state);
        _playerBody.Die();
        _playerLeg.Die();
        StartCoroutine(FinishDie());

        if (Managers.Scene.CurrentScene is not GameScene) return;
        if(GameScene.PlayerLifeCnt > 0) GameScene.PlayerLifeCnt --;
    }

    public override void OnHit(CreatureState state)
    {
        _playerBody.Hit();
    }
    
    public void OnAttack(CreatureState prevState)
    {
        if(prevState != CreatureState.Attack)
            _wpc.Attack(realDamage);
    }

    public void EquipWeapon(int itemId)
    {
        Managers.Resource.Destroy(_wpc.gameObject);
        _wpc = Managers.Resource.Instantiate($"Objects/Character/Weapon/{itemId}", parent: this.transform).GetComponent<WeaponPivotController>();
        _wpc.pivot = _weapon_pivot;
    }
    
    public override void OnDamage(float damage)
    {
        if (!Managers.Network.IsHost) return;
        
        if(IsDead) return;
        base.OnDamage(damage);
        if(State == CreatureState.Idle || State == CreatureState.Run) 
            State = CreatureState.Hit;
    }
    #endregion

    public void BindingAction()
    {
        Managers.Input.PlayerActions.Move.AddEvent(OnMoveInput);
        Managers.Input.PlayerActions.Attack.AddEvent(OnAttackInput);
        Managers.Input.PlayerActions.Interact.AddEvent(OnInteract);
    }

    private void Flip(bool isFlip)
    {
        Left_Arm.SetActive(isFlip);
        Right_Arm.SetActive(!isFlip);
        Vector2 localSc = transform.localScale;
        localSc.x = isFlip ? -1 * Math.Abs(localSc.x) : localSc.x = Math.Abs(localSc.x);
        transform.localScale = localSc;
        _name.transform.localScale = localSc;
        HPText.transform.localScale = localSc;
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

    ///
    ///<summary>
    ///공격 입력이 들어왔을 때 가장 먼저 호출되는 함수
    ///</summary>
    private void OnAttackInput(InputAction.CallbackContext context)
    {
        if(IsDead) return;
        if(!context.started) return;
        if(Managers.Network.LocalPlayer != this) return;
        if(State != CreatureState.Idle && State != CreatureState.Attack && State != CreatureState.Run) return;
        
        // //이동중간에 액션 들어올 경우를 대비해서, 공격 시작 시 위치 고정
        // rb.velocity = Vector2.zero;

        if( State == CreatureState.Attack)
            _attackInputBuffer = true;

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
        
    }

    private IEnumerator FinishDie()
    {
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(false);

        if (Managers.Network.LocalPlayer == this)
        {
            if (Camera.main != null) playerCameraController = Camera.main.GetComponent<PlayerCameraController>();
            if (!playerCameraController.enabled)
                playerCameraController.enabled = true;
            playerCameraController.SetPosition(transform.position);
        }

        if (!Managers.Network.IsHost) yield break;
        S_DeSpawn despawn = new S_DeSpawn();
        despawn.ObjectIds.Add(Id);
        Managers.Network.Server.Room.Broadcast(despawn);
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
        var isFlip = PosInfo.Dir == -1;
        Left_Arm.SetActive(isFlip);
        Right_Arm.SetActive(!isFlip);
        Vector2 localSc = transform.localScale;
        _name.transform.localScale = localSc;
        HPText.transform.localScale = localSc;
    }

    public override void UpdateHp(float health, bool dead)
    {
        base.UpdateHp(health,dead);
        HPBar.fillAmount = HP / maxHP;
        HPText.text = $"{HP / maxHP:P2}";
    }

    public void SyncWPos(float WposX, float WposY, float WrotZ)
    {
        var t = _wpc.transform;
        t.localPosition = new Vector3(WposX, WposY);
        t.localEulerAngles = new Vector3(0, 0, WrotZ);
    }
}
