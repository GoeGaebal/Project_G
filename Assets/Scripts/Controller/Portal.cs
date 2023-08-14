using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("enter감지!");
        if (Managers.Network.PlayerDict[other.gameObject.GetPhotonView().OwnerActorNr] == Managers.Network.LocalPlayer)
        {
            PhotonNetwork.LeaveRoom();
        }
            // Managers.Scene.LoadScene(Define.Scene.Ship);
    }
}
