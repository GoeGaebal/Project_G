using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NetworkManager : IOnEventCallback
{
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    
    public void Init()
    {
        OnEnable();
    }

    public void SpawnPlayer()
    {
        Vector3 SpawnPos = Vector3.zero;
        GameObject player = PhotonNetwork.Instantiate("Prefabs/Player", SpawnPos, Quaternion.identity);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 1)
        {
            byte[] data = (byte[])photonEvent.CustomData;
            Managers.Object.DeserializeDictionary(data);
            Managers.Object.SpawningList();
            
        }
        else if (eventCode == 2)
        {
            Managers.Object.SendDictionaryToClients();
        }
    }
}
