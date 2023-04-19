using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[System.Serializable]
public class ObjectInfo
{
    public int id;
    public int guid;
    public Vector3 pos;
}

public class NetworkManager : IOnEventCallback, IPunObservable
{

    public GameObject LocalPlayer { get; private set; }
    public Dictionary<int, ObjectInfo> ObjectInfos { get; private set; }
    public Dictionary<int, GameObject> LocalObjectsDict { get; private set; }
    
    // If you have multiple custom events, it is recommended to define them in the used class
    private const byte SendMasterToClientsEventCode = 1;
    private const byte SendClientToMasterEventCode = 2;
    private const byte SendObjectDictEventCode = 3;
    private const byte ReceiveObjectDictEventCode = 4;

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
        ObjectInfos = new Dictionary<int, ObjectInfo>();
        LocalObjectsDict = new Dictionary<int, GameObject>();
    }

    public void SpawnLocalPlayer(Vector3 spawnPos = default(Vector3))
    {
        LocalPlayer = PhotonNetwork.Instantiate("Prefabs/Player", spawnPos, Quaternion.identity);
    }
    
    private GameObject SpawnGathering(int objectId, Vector3 spawnPos)
    {
        var name = Managers.Data.GatheringDict[objectId].name;
        GameObject go = Managers.Resource.Instantiate($"Gathering/{name}", spawnPos, Quaternion.identity);
        return go;
    }
    
    public void SpawnGatherings(int objectId, int count, Vector3[] spawnPoses = null)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (spawnPoses == null)
            {
                for (int i = 0; i != count; i++)
                {
                    Vector3 randPos = Managers.Map.GenerateCurrentRandPos();
                    GameObject go = SpawnGathering(objectId,randPos);
                    int guid = go.GetInstanceID();
                    ObjectInfos.Add(go.GetInstanceID(),new ObjectInfo(){id=objectId,guid=go.GetInstanceID(),pos = randPos});
                    LocalObjectsDict.Add(go.GetInstanceID(),go);
                }
            }
            else
            {
                foreach (Vector3 spawnPos in spawnPoses)
                {
                    GameObject go = SpawnGathering(objectId, spawnPos);
                    ObjectInfos.Add(go.GetInstanceID(),new ObjectInfo(){id=objectId,guid=go.GetInstanceID(),pos = spawnPos});
                    LocalObjectsDict.Add(go.GetInstanceID(),go);
                }
                for (int i = 0; i < count - spawnPoses.Length; i++)
                {
                    Vector3 randPos = Managers.Map.GenerateCurrentRandPos();
                    GameObject go = SpawnGathering(objectId,randPos);
                    ObjectInfos.Add(go.GetInstanceID(),new ObjectInfo(){id=objectId,guid=go.GetInstanceID(),pos = randPos});
                    LocalObjectsDict.Add(go.GetInstanceID(),go);
                }
            }
        }
        else
        {
            SendSignalToMaster(ReceiveObjectDictEventCode);
        }
    }
    
    /// <summary>
    /// 아이템을 뿌려주는 메소드
    /// </summary>
    /// <param name="count">아이템을 촘 몇개 뿌릴 것인지</param>
    /// <param name="spawnPos">뿌려지는 시작 장소</param>
    /// <param name="maxRadious">최대 거리</param>
    /// <param name="minRadious">최소 거리</param>
    public void SpawnLootingItems(int objectId, int count, Vector3 spawnPos, float maxRadious = 10.0f,float minRadious = 0.0f)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 randPos = Random.insideUnitCircle * Random.Range(minRadious,maxRadious);
                randPos.x += spawnPos.x;
                randPos.y += spawnPos.y;
                var name = Managers.Data.LootingDict[objectId].name;
                GameObject go =  Managers.Resource.Instantiate($"Lootings/{name}", spawnPos, Quaternion.identity);
            
                // TODO : GetComponent라는 비싼 연산을 수행하며 시간과 거리가 하드코딩 되어있음.
                // 아마 오브젝트 풀링을 적용하면 조금은 해결될 수도
                LootingItemController lc = go.GetComponent<LootingItemController>();
                lc?.Bounce(randPos, 1.0f, 1.0f);
            }
        }
        else
        {
            SendSignalToMaster(ReceiveObjectDictEventCode);
        }
       
    }

    public void ReceiveInfoDict(Dictionary<int, ObjectInfo> infos)
    {
        foreach (KeyValuePair<int,ObjectInfo> info in infos)
        {
            // SpawnGathering(info.Value.id,info.Value.pos,info.Value.guid);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        var eventCode = photonEvent.Code;
        switch (eventCode)
        {
            case SendMasterToClientsEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                break;
            }
            case SendClientToMasterEventCode:
                BroadCastClients(Serialize(LocalObjectsDict), SendMasterToClientsEventCode);
                break;
            case SendObjectDictEventCode:
                BroadCastClients(Serialize(LocalObjectsDict),ReceiveObjectDictEventCode);
                break;
            case ReceiveObjectDictEventCode:
            {
                var data = (byte[])photonEvent.CustomData;
                ReceiveInfoDict(Deserialize<Dictionary<int, ObjectInfo>>(data));
                break;
            }
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
    
    #region ServerSide

    private static void BroadCastClients(IEnumerable content, byte eventCode)
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion

    #region ClientSide
    public void SendSignalToMaster(byte eventCode)
    {
        RaiseEventOptions raiseEventOptions = new(){ Receivers = ReceiverGroup.MasterClient }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(eventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }
    

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            var data = Serialize(LocalObjectsDict); // serialize the dictionary into a byte array
            stream.SendNext(data); // write the byte array to the PhotonStream
        }
        else
        {
            var data = (byte[])stream.ReceiveNext(); // read the byte array from the PhotonStream
            ObjectInfos = Deserialize<Dictionary<int,ObjectInfo>>(data); // deserialize the byte array into a dictionary
            var infokeys = ObjectInfos.Keys;
            var objkeys = LocalObjectsDict.Keys;
            var toDestoryKeys = objkeys.Except(infokeys);
            var toInstantiateKeys = infokeys.Except(objkeys);
            foreach (var toDestoryKey in toDestoryKeys)
            {
                Managers.Resource.Destroy(LocalObjectsDict[toDestoryKey]);
                LocalObjectsDict.Remove(toDestoryKey);
            }
            foreach (var toInstantiateKey in toInstantiateKeys)
            {
                var go = SpawnGathering(ObjectInfos[toInstantiateKey].id, ObjectInfos[toInstantiateKey].pos);
                LocalObjectsDict.Add(toInstantiateKey,go);
            }
        }
    }
}
