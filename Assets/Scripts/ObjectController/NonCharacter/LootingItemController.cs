using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class LootingItemController : NetworkObject
{
    private Item _item;
    public Item Item
    {
        get => _item;
        set
        {
            // TODO : icon이 인벤토리와 겹침
            _item = value;
            if (_mesh == null)
                _mesh = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _mesh.sprite = value.Icon;
            if (_shadow == null)
                _shadow = transform.GetChild(1).GetComponent<SpriteRenderer>();
            _shadow.sprite = value.Icon;
            if (_item.ID is > 1000 and <= 2000)
            {
                _mesh.transform.localScale = 5 * Vector3.one;
                _shadow.transform.localScale = 5 * Vector3.one;
            }
            else
            {
                _mesh.transform.localScale = Vector3.one;
                _shadow.transform.localScale = Vector3.one;
            }
        }
    }

    private UI_Inven _uiInven;
    private SpriteRenderer _mesh;
    private SpriteRenderer _shadow;
    private CircleCollider2D _collider2D;

    [Header("Physics")]
    [Tooltip("충돌계수")]
    [Range(0.0f,1.0f)]
    [SerializeField] private float cof;
    [Tooltip("임계 속도")]
    [SerializeField] private float threshold;
    private static float Sn;

    protected override void Awake()
    {
        _mesh = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _shadow = transform.GetChild(1).GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<CircleCollider2D>();
        _collider2D.enabled = false;
    }

    private void Start()
    {
        cof = 0.6f;
        threshold = 1.0f;
        Sn = 2.3056f;
        
        _uiInven = FindObjectOfType<UI_Inven>();
        _collider2D.enabled = false;
    }

    public void Bounce(Vector3 endPosition,float duration = 1.0f, float maxHeight = 1.0f)
    {
        StartCoroutine(LazyEnable(2.0f));
        StartCoroutine(CoCalcBouncePos(endPosition, duration, maxHeight));
    }

    private IEnumerator LazyEnable(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        _collider2D.enabled = true;
    }

    private IEnumerator CoCalcBouncePos(Vector3 targetPosition,float duration, float maxHeight)
    {
        // 초기 값 세팅
        float time = 0.0f;
        float curH = 0.0f;
        
        // TODO : 등비수열의 합을 코루틴 시작마다 계산 중임 추후에 데이터베이스 계산된 값을 넣고 불러오도록 하자
        // Sn = (1.0f - Mathf.Pow(cof, bounceCount)) / (1.0f - cof);
        float hSpeed = (4.0f * maxHeight * Sn) / duration;
        float gravity = (2.0f * hSpeed * Sn) / duration;
        Vector3 startPosition = transform.position;
        
        while (time < duration)
        {
            // apply gravity if the entity is in the air:
            if (curH > 0) hSpeed -= gravity * Time.deltaTime;
            // add speed to altitude:
            curH += hSpeed * Time.deltaTime;
            // if the entity did hit the ground:
            if (curH < 0) {
                // simulate a bounce (to avoid negative Z):
                curH = -curH;
                // if the entity was falling down, reduce the speed: 
                if (hSpeed < 0.0f) hSpeed = -hSpeed * cof;
                // if the speed is now below the threshold, snap the entity to the ground:
                if (hSpeed < threshold) {
                    curH = 0.0f;
                    hSpeed = 0.0f;
                }
            }
            
            float t = time / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.up * curH + Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    public void EnableCollider()
    {
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Managers.Network.IsHost) return;

        if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        
        var player = collision.GetComponent<Player>();
        if (player == null) return;
        if (player == Managers.Network.LocalPlayer)
        {
            if (_uiInven.AddItem(Item))
            {
                Debug.Log("아이템 획득 성공");
                S_DeSpawn packet = new S_DeSpawn();
                packet.ObjectIds.Add(Id);
                Managers.Network.Server.Room.Broadcast(packet);
            }
            else
            {
                Debug.Log("아이템 획득 실패");
            }
        }
        else
        {
            // TODO : 이후에 각 클라이언트가 먼저 먹겠다고 패킷을 날리고 서버측에서 가장 빨리 온 녀석에게 주는 방식으로 바꿔야 할듯
            S_AddItem itemPacket = new S_AddItem
            {
                ObjectId = Id,
                ItemId = Item.ID
            };
            player.Session.Send(itemPacket);
            S_DeSpawn packet = new S_DeSpawn();
            packet.ObjectIds.Add(Id);
            Managers.Network.Server.Room.Broadcast(player.Id, packet);
        }
    }
}
