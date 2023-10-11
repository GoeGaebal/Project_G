using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public bool isExitPortal;
    private int incomingObjectCount = 0;

    private SpriteRenderer _sprite;
    [SerializeField] private bool _movable = true;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Managers.Network.isHost) return;
        Player player = null;
        if (!other.TryGetComponent<Player>(out player)) return;
        
        incomingObjectCount++;
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
                    foreach (Player p in Managers.Network.Server.Room.Players.Values)
                    {
                        DontDestroyOnLoad(p.gameObject);
                    }
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

    private void OnTriggerExit2D(Collider2D other)
    {
        incomingObjectCount--;
    }
}
