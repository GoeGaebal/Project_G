using Google.Protobuf.Protocol;
using UnityEngine;


public class WeaponPivotController : NetworkObject
{
    [Tooltip("회전 반경")]
    [SerializeField] private float disFromBody;
    [Tooltip("회전 중심점")]
    [SerializeField] private Transform pivot;
    private Player _player;

    private Animator _animator;
    private Weapon _weapon;
    private BoxCollider2D _collider;
    private ContactFilter2D _filter2D = new ContactFilter2D();
    private Collider2D[] _results = new Collider2D[10];
    private static readonly int AttackAnimParam = Animator.StringToHash("attack");

    protected override void Awake()
    {
        _player = transform.parent.gameObject.GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _filter2D.useLayerMask = true;
        _filter2D.useTriggers = true;
        _filter2D.SetLayerMask(LayerMask.GetMask("Monster"));
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

    public void Attack(float realDamage)
    {
        _animator.SetTrigger(AttackAnimParam);
        if (!Managers.Network.IsHost) return;
        
        // 실제 데미지 함수
        var numResults = Physics2D.OverlapCollider(_collider,_filter2D,_results);
        for (var i = 0; i < numResults; i++)  _results[i].gameObject.GetComponent<CreatureController>()?.OnDamage(realDamage);
    }

    public void OnEndAttack()
    {
        _player.FinishAttackState();
    }
    
    
}
