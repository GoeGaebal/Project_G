using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
public class NetworkManager : MonoBehaviourPun, IOnEventCallback, IInRoomCallbacks , IPunOwnershipCallbacks , IPunObservable
{
    public const int MaxPlayer = 3;
    public Player LocalPlayer { get; private set; }
    public Dictionary<int, Player> PlayerDict { get; private set; }
    
    private Queue<Player> _playerQueue;
    private GameObject[] _playerList;
    
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
        EnterRoom,

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
        photonView.ObservedComponents ??= new List<Component>();
        photonView.ObservedComponents.Add(this);
        _playerQueue = new();
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
            case (byte)CustomRaiseEventCode.EnterRoom:
            {
                var data = (byte[])photonEvent.CustomData;
                List<int> stream = Deserialize<List<int>>(data);
                int enteredActor = stream[^2];
                int allocatedView = stream[^1];

                if (enteredActor == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    int idx = 0;
                    foreach (var player in _playerQueue.ToArray())
                    {
                        PhotonView[] views = player.PhotonViews;
                        foreach(var view in views)
                        {
                            view.ViewID = stream[idx++];
                        }
                    }
                    int count =  stream[idx];
                    for (int i = 0; i < count; i++)
                    {
                        int actorNr = stream[idx + 2 * i + 1];
                        int viewId = stream[idx + 2 * i + 2];
                        Player onlinePlayer = TakeoutPlayerQueue(viewId);
                        PlayerDict.Add(actorNr, onlinePlayer);
                        onlinePlayer.gameObject.SetActive(true);
                    }
                }
                PlayerDict.Add(enteredActor,TakeoutPlayerQueue(allocatedView));
                PlayerDict[enteredActor].gameObject.SetActive(true);
                PlayerDict[enteredActor].transform.position = Vector3.zero;
                if (enteredActor == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    LocalPlayer = PlayerDict[enteredActor];
                    PhotonView[] views = LocalPlayer.PhotonViews;
                    foreach(var view in views)
                        view.RequestOwnership();
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

    public void InitRoom()
    {
        _playerList = new []{Managers.Resource.Instantiate("Player"), Managers.Resource.Instantiate("Player"), Managers.Resource.Instantiate("Player")};
        // TODO: 현재 나머지 ViewId는 직접 만들어서 쓰나, 맨 처음 ActorNumber가 1이고 1001은 Managers에 넘어가므로 나머지를 자동 할당한다는 느낌으로 만들고 있음.
        int manualViewId = 1002;
        foreach (var player in _playerList)
        {
            PhotonView[] views = player.GetComponent<Player>().PhotonViews;
            foreach (var view in views)
            {
                view.OwnershipTransfer = OwnershipOption.Request;
                if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) PhotonNetwork.AllocateViewID(view);
                else view.ViewID = manualViewId++;
            }
            _playerQueue.Enqueue(player.GetComponent<Player>());
            player.SetActive(false);
            DontDestroyOnLoad(player);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            var player = TakeoutPlayerQueue();
            player.gameObject.SetActive(true);
            LocalPlayer = player;
            PlayerDict.Add(PhotonNetwork.LocalPlayer.ActorNumber, player);
            LocalPlayer.BindingAction();
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

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            List<int> stream = new();
            foreach (var player in _playerList)
            {
                PhotonView[] views = player.GetPhotonViewsInChildren();
                foreach (var view in views)
                {
                    stream.Add(view.ViewID);
                }
            }
            stream.Add(PlayerDict.Count);
            foreach (var onlinePlayer in PlayerDict)
            {
                stream.Add(onlinePlayer.Key);
                stream.Add(onlinePlayer.Value.photonView.ViewID);
            }
            stream.Add(newPlayer.ActorNumber);
            stream.Add(_playerQueue.Peek().photonView.ViewID);
            PhotonNetwork.RaiseEvent((byte)CustomRaiseEventCode.EnterRoom, Serialize(stream), new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });
        }
    }

    private Player TakeoutPlayerQueue(int viewId = 0)
    {
        if (viewId == 0) return _playerQueue.Dequeue();

        for (int i = 0; i < _playerQueue.Count; i++)
        {
            if (_playerQueue.Peek().photonView.ViewID == viewId) return _playerQueue.Dequeue();
            _playerQueue.Enqueue(_playerQueue.Dequeue());
        }
        return null;
    }

    public void LeftRoom()
    {
        PlayerDict.Clear();
        _playerQueue.Clear();
        foreach (var player in _playerList)
        {
            Managers.Resource.Destroy(player);
        }
        photonView.ViewID = 0;
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // Managers.Resource.Destroy(PlayerDict[otherPlayer.ActorNumber].gameObject);
        _playerQueue.Enqueue(PlayerDict[otherPlayer.ActorNumber]);
        PlayerDict[otherPlayer.ActorNumber].gameObject.SetActive(false);
        PlayerDict.Remove(otherPlayer.ActorNumber);
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
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //stream.SendNext();
            }
        }
    }

    public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
    {
        targetView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
    {
        Debug.Log($"Success to Transfer previousOwner {previousOwner} to {targetView.Owner}");
        if (targetView.Owner.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) return;
        
        var player = targetView.gameObject.GetComponent<Player>();
        if(player != null)
            player.BindingAction();
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
    {
        Debug.Log($"Failed to Transfer previousOwner {senderOfFailedRequest} to {targetView.Owner}");
    }
}
