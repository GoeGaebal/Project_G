using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
        incomingObjectCount++;
        if (isExitPortal && Managers.Network.PlayerDict[other.gameObject.GetPhotonView().OwnerActorNr] == Managers.Network.LocalPlayer)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount <= incomingObjectCount)
            {
                if (SceneManager.GetActiveScene().name == "Lobby")
                {
                    Managers.Scene.LoadScene(Define.Scene.Ship);
                }
                else if (SceneManager.GetActiveScene().name == "Ship")
                {
                    Managers.Scene.LoadScene(Define.Scene.Game);
                }
                else if (SceneManager.GetActiveScene().name == "Game")
                {
                    Managers.Scene.LoadScene(Define.Scene.Ship);
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
