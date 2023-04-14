using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : IOnEventCallback
{
    public GameObject LocalPlayer { get; private set; }
    
    // If you have multiple custom events, it is recommended to define them in the used class
    private const byte SendMasterToClientsEventCode = 1;
    private const byte SendClientToMasterEventCode = 2;

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

    public void SpawnLocalPlayer(Vector3 spawnPos = default(Vector3))
    {
        LocalPlayer = PhotonNetwork.Instantiate("Prefabs/Player", spawnPos, Quaternion.identity);
    }

    public void OnEvent(EventData photonEvent)
    {
        var eventCode = photonEvent.Code;
        switch (eventCode)
        {
            case SendMasterToClientsEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                Managers.Object.SpawningList(Deserialize<Dictionary<int, ObjectManager.ObjectInfo>>(data));
                break;
            }
            case SendClientToMasterEventCode:
                BroadCastClients(Serialize(Managers.Object.ObjectsDict));
                break;
        }
    }

    private static IEnumerable<byte> Serialize<T>(T data)
    {
        BinaryFormatter bf = new();
        MemoryStream ms = new();
        bf.Serialize(ms, data);
        return ms.ToArray();
    }

    private static T Deserialize<T>(byte[] data)
    {
        BinaryFormatter bf = new();
        MemoryStream ms = new(data);
        return (T)bf.Deserialize(ms);
    }
    
    #region 서버측

    private static void BroadCastClients(IEnumerable content)
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(SendMasterToClientsEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion

    #region 클라측
    public void SendSignalToMaster()
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.MasterClient }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(SendClientToMasterEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }
    

    #endregion
}
