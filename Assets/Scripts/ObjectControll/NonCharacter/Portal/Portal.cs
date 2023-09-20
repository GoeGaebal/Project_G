using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public bool isExitPortal;
    public int players;
    private int incomingObjectCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        incomingObjectCount++;
        if (isExitPortal && Managers.Network.PlayerDict[other.gameObject.GetPhotonView().OwnerActorNr] == Managers.Network.LocalPlayer)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (players <= incomingObjectCount)
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

    private void OnTriggerExit2D(Collider2D other)
    {
        incomingObjectCount--;
    }
}
