using System;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Portal : MonoBehaviour
{
    public bool isExitPortal;
    private static ContactFilter2D _filter2D;

    private Collider2D _collider;
    private readonly Collider2D[] _results = new Collider2D[5];

    private SpriteRenderer _sprite;
    [SerializeField] private bool movable = true;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _filter2D.useLayerMask = true;
        _filter2D.useTriggers = true;
        _filter2D.SetLayerMask(LayerMask.GetMask("Player"));
    }

    private void Update()
    {
        if (!Managers.Network.IsHost) return;
        var incomingObjectCount = Physics2D.OverlapCollider(_collider,_filter2D,_results);
        if (incomingObjectCount == 0) return;
        if (isExitPortal)
        {
            Managers.Network.Server.Room.LeaveGame(_results[0].GetComponent<Player>().Info.ObjectId);
            return;
        }

        if (!movable || Managers.Network.Server.Room.PlayersCount > incomingObjectCount) return;
        switch (Managers.Scene.CurrentScene.SceneType)
        {
            case SceneType.Lobby:
                // foreach (Player p in Managers.Network.Server.Room.Players.Values)
                // {
                //     DontDestroyOnLoad(p.gameObject);
                // }
                Managers.Network.Server.Room.LoadScene(SceneType.Ship);
                break;
            case SceneType.Ship:
                Managers.Network.Server.Room.LoadScene(SceneType.Game);
                break;
            case SceneType.Game:
                Managers.Network.Server.Room.LoadScene(SceneType.Ship);
                break;
            case SceneType.Unknown:
                break;
            case SceneType.FinalBoss:
                break;
            case SceneType.Loading:
                break;
            case SceneType.Credit:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        movable = false;
    }

    public void SetColor(Color color)
    {
        _sprite.color = color;
    }

    public void SetPortal(bool move)
    {
        movable = move;
    }
}
