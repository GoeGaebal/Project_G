using System;
using System.Security.Cryptography.X509Certificates;
using ExitGames.Client.Photon.StructWrapping;
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
    private Animator _animator;
    private bool _isAnimating;
    [SerializeField] private bool startOpen;
    private bool Movable
    {
        get => _animator.GetBool(IsOpen);
        set => _animator.SetBool(IsOpen, value);
    }
    
    
    private static readonly int OpenTrigger = Animator.StringToHash("Open");
    private static readonly int CloseTrigger = Animator.StringToHash("Close");
    private static readonly int IsOpen = Animator.StringToHash("isOpen");

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _filter2D.useLayerMask = true;
        _filter2D.useTriggers = true;
        _filter2D.SetLayerMask(LayerMask.GetMask("Player"));
        
        _animator.ResetTrigger(OpenTrigger);
        _animator.ResetTrigger(CloseTrigger);
        Movable = startOpen;
        _isAnimating = false;
    }

    private void Open()
    {
        if (Movable || _isAnimating) return;
        _animator.SetTrigger(OpenTrigger);
        _isAnimating = true;
    }

    private void Close()
    {
        if (!Movable || _isAnimating) return;
        _animator.SetTrigger(CloseTrigger);
        _isAnimating = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (!Managers.Network.IsHost) return;
        CheckPlayers();
    }

    private void CheckPlayers()
    {
        var incomingObjectCount = Physics2D.OverlapCollider(_collider,_filter2D,_results);
        if (incomingObjectCount == 0) return;
        
        if (isExitPortal)
        {
            Managers.Network.Server.Room.LeaveGame(_results[0].GetComponent<Player>().Info.ObjectId);
            return;
        }
        
        if (!Movable || Managers.Network.Server.Room.PlayersCount > incomingObjectCount) return;
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
    }

    private void PortalOpened()
    {
        Movable = true;
        _isAnimating = false;
        CheckPlayers();
    }

    private void PortalClosed()
    {
        Movable = false;
        _isAnimating = false;
    }

    public void SetPortal(bool move)
    {
        if (move) Open();
        else Close();
    }
}
