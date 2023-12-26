using System;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    public bool isExitPortal;
    private static ContactFilter2D _filter2D;

    private Collider2D _collider;
    private readonly Collider2D[] _results = new Collider2D[5];
    
    private static readonly int OpenTrigger = Animator.StringToHash("Open");
    private static readonly int CloseTrigger = Animator.StringToHash("Close");
    private static readonly int IsOpen = Animator.StringToHash("isOpen");

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _filter2D.useLayerMask = true;
        _filter2D.useTriggers = true;
        _filter2D.SetLayerMask(LayerMask.GetMask("Player"));
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
        Managers.Network.Server.Room.LeaveGame(_results[0].GetComponent<Player>().Info.ObjectId);
    }
}
