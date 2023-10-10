using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class ObjectInfo
{
    public int objectID;
    public int guid;
    public float y;
    public float x;
}

public class ObjectManager
{
    public Player LocalPlayer;
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    public Dictionary<int, Player> Players = new Dictionary<int, Player>();
    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    private int _counter = 0;
    
    public int GenerateId(GameObjectType type)
    {
        return ((int)type << 24) | (_counter++);
    }
    
    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public void Add(Google.Protobuf.Protocol.ObjectInfo info, bool myPlayer = false)
    {
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        if (objectType == GameObjectType.Player)
        {
            
            if (myPlayer)
            {
                // TODO : 만일 호스트이면 이미 추가했을 것이기에 체크하는 것, 호스트 모드를 따로 만들 것인지 아니면 아예 서버랑 분리시킬 것인지 고려해야 함
                Player p = null;
                if (Players.TryGetValue(info.ObjectId, out p)) LocalPlayer = p;
                else
                {
                    GameObject go = Managers.Resource.Instantiate("Objects/Character/Player");
                    LocalPlayer = go.GetComponent<Player>();
                    Players.Add(info.ObjectId, LocalPlayer);
                }
                
                _objects.Add(info.ObjectId, LocalPlayer.gameObject);
                LocalPlayer.gameObject.name = info.Name;
                LocalPlayer.Id = info.ObjectId;
                // LocalPlayer.PosInfo = info.PosInfo;
                LocalPlayer.BindingAction();
                

                
                // LocalPlayer.Stat = info.StatInfo;
                // LocalPlayer.SyncPos();
            }
            else
            {
                GameObject go = null;
                if (Players.ContainsKey(info.ObjectId)) go = Players[info.ObjectId].gameObject;
                else
                {
                    go = Managers.Resource.Instantiate("Objects/Character/Player");
                    Players.Add(info.ObjectId, go.GetComponent<Player>());
                }
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                // pc.Id = info.ObjectId;
                // pc.PosInfo = info.PosInfo;
                // pc.Stat = info.StatInfo;
                // pc.SyncPos();
            }
        }
    }

    public bool Remove(int objectId)
    {
        GameObjectType objectType = GetObjectTypeById(objectId);

        if (objectType == GameObjectType.Player)
            return Players.Remove(objectId);

        return false;
    }


    #region PUN
    public Dictionary<int, ObjectInfo> ObjectInfos { get; private set; }
    public Dictionary<int, GameObject> LocalObjectsDict { get; private set; }

    private System.Random rand = new System.Random();

    public void Init()
    {
        ObjectInfos = new Dictionary<int, ObjectInfo>();
        LocalObjectsDict = new Dictionary<int, GameObject>();
    }
    
    private void SpawnGathering(int objectId, Vector3 pos, int guid = 0)
    {
        var name = Managers.Data.GatheringDict[objectId].name;
        GameObject go = Managers.Resource.Instantiate($"Objects/NonCharacter/Gathering/{name}", pos, Quaternion.identity);
        GatheringController ga =  go.GetOrAddComponent<GatheringController>();
        if (guid == 0)
            ga.guid = go.GetInstanceID();
        else
            ga.guid = guid;

        ga.id = objectId;
        LocalObjectsDict.Add(ga.guid,go);
        ObjectInfos.Add(ga.guid, new ObjectInfo(){objectID=objectId,guid=ga.guid,y = pos.y,x = pos.x});
    }

    public void SpawnGatherings(int objectId, int count, Vector3[] spawnPoses = null)
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            if (Managers.Map.currentMapId == 1)
            {
                SpawnGathering(objectId, new Vector3(16.1329117f, 16.933033f, 0));
                SpawnGathering(objectId, new Vector3(1.83291054f, 33.3330345f, 0));
                SpawnGathering(objectId, new Vector3(29.0429115f, -2.95696831f, 0));
                SpawnGathering(objectId, new Vector3(41.2329102f, 45.5330353f, 0));
                SpawnGathering(objectId, new Vector3(67.7329102f, 39.433033f, 0));
            }
            else if (spawnPoses == null)
            {
                for (int i = 0; i != count; i++)
                {
                    Vector3 randPos = Managers.Map.GenerateCurrentRandPos();
                    SpawnGathering(objectId,randPos);
                }
            }
            else
            {
                foreach (Vector3 spawnPos in spawnPoses)
                {
                    SpawnGathering(objectId, spawnPos);
                }
                for (int i = 0; i < count - spawnPoses.Length; i++)
                {
                    Vector3 randPos = Managers.Map.GenerateCurrentRandPos();
                    SpawnGathering(objectId,randPos);
                }
            }
        }
        else
        {
            Managers.Network.RequestGatherings();
        }
    }

    // TODO: 생성시에 최종적으로 등장하는 랜덤한 위치를 서로 공유해야함
    private LootingItemController SpawnLootingItem(int objectId, Vector3 pos, int guid = 0)
    {
        
        GameObject go =  Managers.Resource.Instantiate($"Objects/NonCharacter/Lootings/apple", pos, Quaternion.identity);
        LootingItemController lc = go.GetOrAddComponent<LootingItemController>();
        
        if (guid == 0)
            lc.guid = go.GetInstanceID();
        else
            lc.guid = guid;
        
        var item = Managers.Data.ItemDict[objectId];
        lc.Item = item;
        ObjectInfos.Add(lc.guid, new ObjectInfo(){objectID=objectId,guid=lc.guid,y = pos.y,x = pos.x});
        LocalObjectsDict.Add(lc.guid, go);
        return lc;
    }

    /// <summary>
    /// 아이템을 뿌려주는 메소드
    /// </summary>
    /// <param name="objectId">아이템 번호</param>
    /// <param name="count">아이템을 촘 몇개 뿌릴 것인지</param>
    /// <param name="spawnPos">뿌려지는 시작 장소</param>
    /// <param name="maxRadious">최대 거리</param>
    /// <param name="minRadious">최소 거리</param>
    public void SpawnLootingItems(int objectId, int count, Vector3 spawnPos, float maxRadious = 10.0f,float minRadious = 0.0f)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<LootingItemInfo> lootingInfos = new();
            for (int i = 0; i < count; i++)
            {
                Vector2 randPos = Random.insideUnitCircle * Random.Range(minRadious,maxRadious);
                randPos.x += spawnPos.x;
                randPos.y += spawnPos.y;
                
                lootingInfos.Add(new LootingItemInfo(){objectId = objectId ,guid = GenerateGuid(), y = spawnPos.y, x = spawnPos.x, destY = randPos.y, destX = randPos.x});
            }
            Managers.Network.BroadCastLootingInfos(lootingInfos);
        }
        else
        {
            Managers.Network.RequestSpawnLootingItems(objectId,count, spawnPos, maxRadious, minRadious);
        }
    }

    public void ApplySpawnLootingItems(List<LootingItemInfo> infos)
    {
        foreach (var info in infos)
        {
            LootingItemController lc = SpawnLootingItem(info.objectId, new Vector3(info.x, info.y), info.guid);
            // TODO : GetComponent라는 비싼 연산을 수행하며 시간과 거리가 하드코딩 되어있음.
            // 아마 오브젝트 풀링을 적용하면 조금은 해결될 수도
            lc?.Bounce(new Vector3(){x=info.destX, y= info.destY}, 1.0f, 1.0f);
        }
    }

    /// <summary>
    /// 랜덤으로 int형 guid 값을 만드는 함수, 만일 모든 int형을 다 쓰면 무한루프를 돌 것이다.
    /// </summary>
    /// <returns>int형 랜덤값</returns>
    public int GenerateGuid()
    {
        if (LocalObjectsDict == null)
            return -1;
        
        int guid = 0;
        while (guid == 0 || LocalObjectsDict.ContainsKey(guid))
            guid = rand.Next();

        return guid;
    }

    public void SyncronizeObjects(Dictionary<int, ObjectInfo> infos)
    {
        var infokeys = infos.Keys;
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
            SpawnGathering(infos[toInstantiateKey].objectID, new Vector3(infos[toInstantiateKey].x,infos[toInstantiateKey].y),toInstantiateKey );
        }
    }
    #endregion
}
