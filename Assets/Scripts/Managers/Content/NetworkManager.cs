using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : IOnEventCallback, IPunObservable
{
    public PhotonView View { get; private set; }
    public GameObject LocalPlayer { get; private set; }

    public Action<string> ReceiveChatHandler;
    
    private static readonly RaiseEventOptions SendMasterOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
    private static readonly RaiseEventOptions SendClientOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

    // If you have multiple custom events, it is recommended to define them in the used class
    private const byte RequestObjectInfosEventCode = 1;
    private const byte ReceiveObjectInfosEventCode = 2;
    private const byte RequestViewIDEventCode = 3;
    private const byte ReceiveViewIDEventCode = 4;
    private const byte ReceiveChatEventCode = 5;

    #region Network
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    
    public void Init(PhotonView inPhotonView)
    {
        OnEnable();
        View = inPhotonView;
    }

    public void AllocateViewId()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            PhotonNetwork.AllocateViewID(View);
        else
            PhotonNetwork.RaiseEvent(RequestViewIDEventCode, null, SendMasterOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        var eventCode = photonEvent.Code;
        switch (eventCode)
        {
            case RequestObjectInfosEventCode:
            {
                BroadCastClients(Serialize(Managers.Object.ObjectInfos), ReceiveObjectInfosEventCode);
                break;
            }
            case ReceiveObjectInfosEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                Managers.Object.SyncronizeObjects(Deserialize<Dictionary<int, ObjectInfo>>(data));
                break;
            }
            case RequestViewIDEventCode:
            {
                BroadCastClients(Serialize(View.ViewID), ReceiveViewIDEventCode);
                break;
            }
            case ReceiveViewIDEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                if (data != null)
                {
                    var viewId = Deserialize<int>(data);
                    if (View.ViewID != viewId)
                        View.ViewID = viewId;
                }
                break;
            }
            case ReceiveChatEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                if (ReceiveChatHandler != null)
                    ReceiveChatHandler.Invoke(Deserialize<string>(data));
                break;
            }
        }
    }
    
    #region ServerSide
    private static void BroadCastClients(IEnumerable content, byte eventCode)
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion
    
    #region ClientSide
    public void RequestGatherings()
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.MasterClient }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(RequestObjectInfosEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion

    public void SendChat(string text)
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ReceiveChatEventCode, Serialize(text), raiseEventOptions, SendOptions.SendReliable);
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
    #endregion
    
    public void SpawnLocalPlayer(Vector3 spawnPos = default(Vector3))
    {
        LocalPlayer = PhotonNetwork.Instantiate("Prefabs/Player", spawnPos, Quaternion.identity);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // if (stream.IsWriting)
        // {
        //     var data = Serialize(LocalObjectsDict); // serialize the dictionary into a byte array
        //     stream.SendNext(data); // write the byte array to the PhotonStream
        // }
        // else
        // {
        //     var data = (byte[])stream.ReceiveNext(); // read the byte array from the PhotonStream
        //     ObjectInfos = Deserialize<Dictionary<int,ObjectInfo>>(data); // deserialize the byte array into a dictionary
        //     var infokeys = ObjectInfos.Keys;
        //     var objkeys = LocalObjectsDict.Keys;
        //     var toDestoryKeys = objkeys.Except(infokeys);
        //     var toInstantiateKeys = infokeys.Except(objkeys);
        //     foreach (var toDestoryKey in toDestoryKeys)
        //     {
        //         Managers.Resource.Destroy(LocalObjectsDict[toDestoryKey]);
        //         LocalObjectsDict.Remove(toDestoryKey);
        //     }
        //     foreach (var toInstantiateKey in toInstantiateKeys)
        //     {
        //         var go = SpawnGathering(ObjectInfos[toInstantiateKey].objectID, ObjectInfos[toInstantiateKey].pos);
        //         LocalObjectsDict.Add(toInstantiateKey,go);
        //     }
        // }
    }
}
