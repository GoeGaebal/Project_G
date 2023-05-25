using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Object = UnityEngine.Object;

[System.Serializable]
public struct Packet
{
    public int state;
    public int guid;
    public float damage;
}

[System.Serializable]
public struct RequestSpawnLootingsPacket
{
    public int objectId;
    public int count;
    public float y;
    public float x;
    public float maxRadious;
    public float minRadious;

}

[System.Serializable]
public struct LootingPacket
{
    public int viewId;
    public int guid;
}

[System.Serializable]
public struct LootingItemInfo
{
    public int objectId;
    public int guid;
    public float y;
    public float x;
    public float destY;
    public float destX;
}


public class NetworkManager : IOnEventCallback, IPunObservable
{
    public PhotonView View { get; private set; }
    public GameObject LocalPlayer { get; private set; }

    public Action<string> ReceiveChatHandler;
    public Action<int> ReceiveAddItemHandler;

    private static readonly RaiseEventOptions SendMasterOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
    private static readonly RaiseEventOptions SendClientOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

    // If you have multiple custom events, it is recommended to define them in the used class
    private const byte RequestObjectInfosEventCode = 1;
    private const byte ReceiveObjectInfosEventCode = 2;
    private const byte RequestViewIDEventCode = 3;
    private const byte ReceiveViewIDEventCode = 4;
    private const byte ReceiveChatEventCode = 5;
    private const byte ReceiveGatheringPacketEventCode = 7;
    private const byte ReceiveLootingsEventCode = 8;
    private const byte ReceiveAddItemEventCode = 9;
    private const byte ReceiveSpawnLootingsEventCode = 10;

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
            case ReceiveGatheringPacketEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                if (data != null)
                    PacketHandler(Deserialize<Packet>(data));
                break;
            }
            case ReceiveLootingsEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                if (data != null)
                    Managers.Object.ApplySpawnLootingItems(Deserialize<List<LootingItemInfo>>(data));
                break;
            }
            case ReceiveAddItemEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                if (data != null)
                {
                    LootingPacket packet = Deserialize<LootingPacket>(data);
                    foreach (var player in Object.FindObjectsOfType<PhotonView>())
                    {
                        if (player.ViewID == packet.viewId && player.IsMine)
                        {
                           if (ReceiveAddItemHandler != null)
                               ReceiveAddItemHandler.Invoke(packet.guid);
                        }
                    } 
                }

                break;
            }
            case ReceiveSpawnLootingsEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                if (data != null)
                {
                    RequestSpawnLootingsPacket packet = Deserialize<RequestSpawnLootingsPacket>(data);
                    Managers.Object.SpawnLootingItems(packet.objectId,packet.count,
                        new Vector3(packet.x, packet.y),
                        packet.maxRadious,
                        packet.minRadious);
                }
            }
                break;
        }
    }
    
    #region ServerSide
    private static void BroadCastClients(IEnumerable content, byte eventCode)
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    
    public void BroadCastGatheringDamaged(int guid, float damage)
    {
        BroadCastClients(Serialize(new Packet() { state = 0, damage = damage, guid = guid }), ReceiveGatheringPacketEventCode);
        
    }
    
    public void BroadCastObjectDestroy(int guid)
    {
        BroadCastClients(Serialize(new Packet() { state = 1, damage = 0.0f, guid = guid }), ReceiveGatheringPacketEventCode);
    }
    
    public void BroadCastLootingInfos(List<LootingItemInfo> infos)
    {
        BroadCastClients(Serialize(infos), ReceiveLootingsEventCode);
    }

    public void SendLootingAddItem(int viewId,int guid)
    {
        BroadCastClients(Serialize(new LootingPacket(){viewId = viewId, guid = guid}), ReceiveAddItemEventCode);
    }
    
    #endregion
    
    #region ClientSide
    public void RequestGatherings()
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.MasterClient }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(RequestObjectInfosEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    public void PacketHandler(Packet packet)
    {
        // damaged
        if (packet.state == 0)
        {
            Managers.Object.LocalObjectsDict[packet.guid].GetComponent<GatheringController>().ApplyDamage(packet.damage);
        }
        // died
        else if (packet.state == 1)
        {
            GatheringController gc = Managers.Object.LocalObjectsDict[packet.guid].GetComponent<GatheringController>();
            if (gc != null)
                gc.ApplyDie();
            else
            {
                LootingItemController lc = Managers.Object.LocalObjectsDict[packet.guid].GetComponent<LootingItemController>();
                if (lc != null)
                    lc.ApplyDie();
            }
        }
    }

    public void RequestSpawnLootingItems(int objectId, int count, float y, float x, float maxRadious = 10.0f,float minRadious = 0.0f)
    {
        RequestSpawnLootingsPacket packet = new RequestSpawnLootingsPacket() { objectId = objectId, count = count, y = y, x = x, maxRadious = maxRadious, minRadious = minRadious};
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.MasterClient }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ReceiveSpawnLootingsEventCode, Serialize(packet), raiseEventOptions, SendOptions.SendReliable);
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
