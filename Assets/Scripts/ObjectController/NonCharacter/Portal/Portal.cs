using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public bool isExitPortal;
    private static ContactFilter2D _filter2D;

    private Collider2D _collider;
    private Collider2D[] _results = new Collider2D[5];

    private SpriteRenderer _sprite;
    [SerializeField] private bool _movable = true;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _filter2D.useLayerMask = true;
        _filter2D.useTriggers = true;
        _filter2D.SetLayerMask(LayerMask.GetMask("Player"));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Managers.Network.IsHost) return;
        if (!other.TryGetComponent<Player>(out var player)) return;
        var incomingObjectCount = Physics2D.OverlapCollider(_collider,_filter2D,_results);
        if (isExitPortal)
        {
            S_LeaveGame packet = new S_LeaveGame();
            player.Session.Send(packet);
            //Managers.Network.Server.Room.LeaveGame(player.Info.ObjectId);
        }
        else
        {
            // TODO : 추후 서버측으로 완전 이전
            if (_movable && Managers.Network.Server.Room.PlayersCount <= incomingObjectCount)
            {
                if (SceneManager.GetActiveScene().name == "Lobby")
                {
                    // foreach (Player p in Managers.Network.Server.Room.Players.Values)
                    // {
                    //     DontDestroyOnLoad(p.gameObject);
                    // }
                    Managers.Network.Server.Room.LoadScene(SceneType.Ship);
                }
                else if (SceneManager.GetActiveScene().name == "Ship")
                {
                    Managers.Network.Server.Room.LoadScene(SceneType.Game);
                }
                else if (SceneManager.GetActiveScene().name == "Game")
                {
                    Managers.Network.Server.Room.LoadScene(SceneType.Ship);
                }
            }
        }
    }

    public void SetColor(Color color)
    {
        _sprite.color = color;
    }

    public void SetPortal(bool move)
    {
        _movable = move;
    }
}
