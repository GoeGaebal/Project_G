using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Xml.Serialization;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

[Serializable]
struct PlayerInfo
{
    public int playerId, weaponPivotId, weaponId;
}

// TODO : 리팩토링 
// 1. 패킷을 큐 형태로 모아 보내기
// 2. Player들을 미리 찾아놓기
public class NetworkManager : MonoBehaviourPun, IOnEventCallback, IInRoomCallbacks, IPunObservable
{
    public Player LocalPlayer { get; private set; }
    public List<Player> PlayerList;
    private PlayerInfo _playerInfo;
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
        InitializeRoom,

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
    
    public void Init()
    {
        OnEnable();
        PlayerDict = new Dictionary<int, Player>();
        PlayerList = new();
        _playerInfo = new PlayerInfo();
    }

    public void AllocateViewId()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            PhotonNetwork.AllocateViewID(photonView);
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
                BroadCastClients(Serialize(photonView.ViewID), (byte)CustomRaiseEventCode.ReceiveViewID);
                break;
            }
            case (byte)CustomRaiseEventCode.ReceiveViewID:
            {
                var data = (byte[])photonEvent.CustomData;
                if (data != null)
                {
                    var viewId = Deserialize<int>(data);
                    if (photonView.ViewID != viewId)
                        photonView.ViewID = viewId;
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
                        ReceiveAddItemHandler?.Invoke(packet.guid);
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
                int actorNumber = (int)data[5];
                GameObject newPlayer = Managers.Resource.Instantiate("Player", (Vector3) data[0], (Quaternion) data[1]);
                PhotonView photonView = newPlayer.GetComponent<PhotonView>();
                PhotonView weaponPivotView = newPlayer.GetComponentInChildren<WeaponPivotController>().gameObject.GetComponent<PhotonView>();
                PhotonView weaponView = newPlayer.GetComponentInChildren<PlayerAttackController>().gameObject.GetComponent<PhotonView>();
                photonView.ViewID = (int) data[2];
                weaponPivotView.ViewID = (int) data[3];
                weaponView.ViewID = (int) data[4];
                PlayerDict.Add(actorNumber, newPlayer.GetComponent<Player>());
                PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).TagObject = newPlayer;
                break;
            }
            case (byte)CustomRaiseEventCode.InitializeRoom:
            {
                var data = (byte[])photonEvent.CustomData;
                List<int> newViews = Deserialize<List<int>>(data);
                int idx = -1;
                foreach (var player in PlayerDict.Values)
                {
                    PhotonView[] views = player.gameObject.GetPhotonViewsInChildren();
                    foreach(var view in views)
                    {
                        view.ViewID = newViews[++idx];
                    }
                }
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
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (MemoryStream ms = new MemoryStream())
        {
            serializer.Serialize(ms, data);
            return ms.ToArray();
        }
    }

    private static T Deserialize<T>(byte[] data)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (MemoryStream ms = new MemoryStream(data))
        {
            return (T)serializer.Deserialize(ms);
        }
    }
    #endregion
    
    public void SpawnLocalPlayer(Vector3 spawnPos = default(Vector3))
    {
        // LocalPlayer = PhotonNetwork.Instantiate("Prefabs/Player", spawnPos, Quaternion.identity).GetComponent<Player>();
        GameObject player = Managers.Resource.Instantiate("Player", spawnPos, Quaternion.identity);

        PhotonView photonView = player.GetComponent<PhotonView>();
        PhotonView weaponPivotView = player.GetComponentInChildren<WeaponPivotController>().gameObject.GetComponent<PhotonView>();
        PhotonView weaponView = player.GetComponentInChildren<PlayerAttackController>().gameObject.GetComponent<PhotonView>();
        if (_playerInfo.playerId != 0)
        {
            photonView.ViewID = _playerInfo.playerId;
            weaponPivotView.ViewID = _playerInfo.weaponPivotId;
            weaponView.ViewID = _playerInfo.weaponId;
        }
        else if (PhotonNetwork.AllocateViewID(photonView) && PhotonNetwork.AllocateViewID(weaponView) && PhotonNetwork.AllocateViewID(weaponPivotView))
        {
            InstantiatePlayer(player.transform.position, player.transform.rotation, photonView.ViewID, weaponView.ViewID, weaponPivotView.ViewID);
            LocalPlayer = player.GetComponent<Player>();
            _playerInfo.playerId = photonView.ViewID ;
            _playerInfo.weaponPivotId = weaponPivotView.ViewID;
            _playerInfo.weaponId = weaponView.ViewID;
            PlayerDict.Add(PhotonNetwork.LocalPlayer.ActorNumber, LocalPlayer); 
        }
        PhotonNetwork.LocalPlayer.TagObject = player;
    }

    private void InstantiatePlayer(Vector3 position, Quaternion rotation, int playerView, int pivotView, int weaponView)
    {
        object[] data = new object[]
        {
            position, rotation, playerView, pivotView, weaponView, PhotonNetwork.LocalPlayer.ActorNumber
        };
        PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.CustomInstantiate, data, SendClientOptions, new SendOptions { Reliability = true });
    }

    public void InitRoom()
    {
        GameObject[] players = new []{Managers.Resource.Instantiate("Player"),Managers.Resource.Instantiate("Player"),Managers.Resource.Instantiate("Player")};
        foreach (var player in players)
        {
            PhotonView[] views = player.GetPhotonViewsInChildren();
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
            {
                foreach (var view in views)
                {
                    PhotonNetwork.AllocateViewID(view);
                }
                PlayerDict.Add(views[0].ViewID, player.GetComponent<Player>());
            }
            PlayerList.Add(player.GetComponent<Player>());
            player.SetActive(false);
        }
    }

    public void DespawnLocalPlayer()
    {
        if (LocalPlayer == null)
            return;
        
        PhotonView photonView = LocalPlayer.GetComponent<PhotonView>();
        PhotonView weaponPivotView = LocalPlayer.GetComponentInChildren<WeaponPivotController>().gameObject.GetComponent<PhotonView>();
        PhotonView weaponView = LocalPlayer.GetComponentInChildren<PlayerAttackController>().gameObject.GetComponent<PhotonView>();
        
        // Managers.Resource.Destroy(LocalPlayer.gameObject);
        _playerInfo.playerId = 0;
        Managers.Input.PlayerActions.Move.Reset();
        Managers.Input.PlayerActions.Attack.Reset();
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

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        List<int> viewIds = new();
        foreach (var player in PlayerDict.Values)
        {
            PhotonView[] views = player.gameObject.GetPhotonViewsInChildren();
            foreach (var view in views)
            {
                viewIds.Add(view.ViewID);
            }
        }
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
            PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.InitializeRoom, Serialize(viewIds), SendClientOptions, new SendOptions { Reliability = true });
        // if(newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
        //     InstantiatePlayer(LocalPlayer.transform.position, LocalPlayer.transform.rotation, _playerInfo.playerId, _playerInfo.weaponPivotId, _playerInfo.weaponId);
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // Managers.Resource.Destroy(PlayerDict[otherPlayer.ActorNumber].gameObject);
        PlayerDict.Remove(otherPlayer.ActorNumber);
        // throw new NotImplementedException();
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // throw new NotImplementedException();
    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        // throw new NotImplementedException();
    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        // throw new NotImplementedException();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // throw new NotImplementedException();
    }
}
