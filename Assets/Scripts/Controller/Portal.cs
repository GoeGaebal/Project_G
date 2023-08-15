using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool isExitPortal;
    public int players;
    private int incomingObjectCount = 0;
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        incomingObjectCount++;
        if (isExitPortal && Managers.Network.PlayerDict[other.gameObject.GetPhotonView().OwnerActorNr] == Managers.Network.LocalPlayer)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (players == incomingObjectCount)
                Managers.Scene.LoadScene(Define.Scene.Ship);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        incomingObjectCount--;
    }
}
