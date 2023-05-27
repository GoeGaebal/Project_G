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

// TODO : 리팩토링 
// 1. 패킷을 큐 형태로 모아 보내기
// 2. Player들을 미리 찾아놓기
public class NetworkManager : IOnEventCallback, IPunObservable
{
    public PhotonView View { get; private set; }
    public Player LocalPlayer { get; private set; }
    public Dictionary<int, Player> PlayerDict { get; private set; }



    public Action<string> ReceiveChatHandler;
    public Action<int> ReceiveAddItemHandler;

    private static readonly RaiseEventOptions SendMasterOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
    private static readonly RaiseEventOptions SendClientOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

    // If you have multiple custom events, it is recommended to define them in the used class
    public enum CustomRaiseEventCode : byte
    {
        RequestObjectInfos,
        ReceiveObjectInfos,
        RequestViewID,
        ReceiveViewID,
        ReceiveChat,
        ReceiveGatheringPacket,
        ReceiveLootings,
        ReceiveAddItem,
        ReceiveSpawnLootings,
        CustomInstantiate,

        SynchronizeTime,
        RequestSynchronizeTime,
    }

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
        PlayerDict = new Dictionary<int, Player>();
        View = inPhotonView;
    }

    public void AllocateViewId()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            PhotonNetwork.AllocateViewID(View);
        else
            PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.RequestViewID, null, SendMasterOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        var eventCode = photonEvent.Code;
        switch (eventCode)
        {
            case (byte)CustomRaiseEventCode.RequestObjectInfos:
            {
                BroadCastClients(Serialize(Managers.Object.ObjectInfos), (byte)CustomRaiseEventCode.ReceiveObjectInfos);
                break;
            }
            case (byte)CustomRaiseEventCode.ReceiveObjectInfos:
            {
                var data = (byte[])photonEvent.CustomData;
                Managers.Object.SyncronizeObjects(Deserialize<Dictionary<int, ObjectInfo>>(data));
                break;
            }
            case (byte)CustomRaiseEventCode.RequestViewID:
            {
                BroadCastClients(Serialize(View.ViewID), (byte)CustomRaiseEventCode.ReceiveViewID);
                break;
            }
            case (byte)CustomRaiseEventCode.ReceiveViewID:
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
            case (byte)CustomRaiseEventCode.ReceiveChat:
            {
                var data = (byte[])photonEvent.CustomData;
                if (ReceiveChatHandler != null)
                    ReceiveChatHandler.Invoke(Deserialize<string>(data));
                break;
            }
            case (byte)CustomRaiseEventCode.ReceiveGatheringPacket:
            {
                var data = (byte[])photonEvent.CustomData;
                if (data != null)
                    PacketHandler(Deserialize<Packet>(data));
                break;
            }
            case (byte)CustomRaiseEventCode.ReceiveLootings:
            {
                var data = (byte[])photonEvent.CustomData;
                if (data != null)
                    Managers.Object.ApplySpawnLootingItems(Deserialize<List<LootingItemInfo>>(data));
                break;
            }
            case (byte)CustomRaiseEventCode.ReceiveAddItem:
            {
                var data = (byte[])photonEvent.CustomData;
                if (data != null)
                {
                    LootingPacket packet = Deserialize<LootingPacket>(data);
                    if (packet.viewId == LocalPlayer.photonView.ViewID)
                    { 
                        if (ReceiveAddItemHandler != null) ReceiveAddItemHandler.Invoke(packet.guid);
                    }
                }

                break;
            }
            case (byte)CustomRaiseEventCode.ReceiveSpawnLootings:
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
                break;
            }
            case (byte)CustomRaiseEventCode.CustomInstantiate:
            {
                object[] data = (object[]) photonEvent.CustomData;
                GameObject player = Managers.Resource.Instantiate("Player", (Vector3) data[0], (Quaternion) data[1]);
                PhotonView photonView = player.GetComponent<PhotonView>();
                PhotonView weaponView = player.GetComponentInChildren<WeaponPivotController>().gameObject.GetComponent<PhotonView>();
                photonView.ViewID = (int) data[2];
                weaponView.ViewID = (int) data[3];
                PlayerDict.Add(photonView.ViewID, LocalPlayer.GetComponent<Player>());
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
    
    public void BroadCastGatheringDamaged(int guid, float damage)
    {
        BroadCastClients(Serialize(new Packet() { state = 0, damage = damage, guid = guid }), (byte)CustomRaiseEventCode.ReceiveGatheringPacket);
        
    }
    
    public void BroadCastObjectDestroy(int guid)
    {
        BroadCastClients(Serialize(new Packet() { state = 1, damage = 0.0f, guid = guid }), (byte)CustomRaiseEventCode.ReceiveGatheringPacket);
    }
    
    public void BroadCastLootingInfos(List<LootingItemInfo> infos)
    {
        BroadCastClients(Serialize(infos), (byte)CustomRaiseEventCode.ReceiveLootings);
    }

    public void SendLootingAddItem(int viewId,int guid)
    {
        BroadCastClients(Serialize(new LootingPacket(){viewId = viewId, guid = guid}), (byte)CustomRaiseEventCode.ReceiveAddItem);
    }
    
    #endregion
    
    #region ClientSide
    public void RequestGatherings()
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.MasterClient }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.RequestObjectInfos, null, raiseEventOptions, SendOptions.SendReliable);
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

    public void RequestSpawnLootingItems(int objectId, int count, Vector3 pos, float maxRadious = 10.0f,float minRadious = 0.0f)
    {
        RequestSpawnLootingsPacket packet = new RequestSpawnLootingsPacket() { objectId = objectId, count = count, y = pos.y, x = pos.x, maxRadious = maxRadious, minRadious = minRadious};
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.MasterClient }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.ReceiveSpawnLootings, Serialize(packet), raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion

    public void SendChat(string text)
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.ReceiveChat, Serialize(text), raiseEventOptions, SendOptions.SendReliable);
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
        // LocalPlayer = PhotonNetwork.Instantiate("Prefabs/Player", spawnPos, Quaternion.identity).GetComponent<Player>();
        GameObject player = Managers.Resource.Instantiate("Player", spawnPos, Quaternion.identity);
        PhotonView photonView = player.GetComponent<PhotonView>();
        PhotonView weaponView = player.GetComponentInChildren<WeaponPivotController>().gameObject.GetComponent<PhotonView>();
        if (PhotonNetwork.AllocateViewID(photonView) && PhotonNetwork.AllocateViewID(weaponView))
        {
            object[] data = new object[]
            {
                player.transform.position, player.transform.rotation, photonView.ViewID, weaponView.ViewID
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };
            PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.CustomInstantiate, data, raiseEventOptions, sendOptions);
            LocalPlayer = player.GetComponent<Player>();
            PlayerDict.Add(photonView.ViewID,LocalPlayer); 
        }
    }
    


    public void SynchronizeTime()
    {
        if(Managers.TimeSlot == null) return;

        if(!PhotonNetwork.IsMasterClient) return;

        Debug.Log("send time event");
        object[] content = new object[] { Managers.TimeSlot.CurrentTime, Managers.TimeSlot.TimeSlot,Managers.TimeSlot.CountTimeSlotChanged, RotateTimer.GetTimerAngle()};
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.Others};  
        PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.SynchronizeTime, content, raiseEventOptions, SendOptions.SendReliable);
    }
    public void RequestSynchronizeTime()
    {
        if(Managers.TimeSlot == null) return;

        if(PhotonNetwork.IsMasterClient) return;

        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.MasterClient};  
        PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.RequestSynchronizeTime, null, raiseEventOptions, SendOptions.SendReliable);
   
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
