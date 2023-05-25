using System.Collections.Generic;
using System.Linq;
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
    public Dictionary<int, ObjectInfo> ObjectInfos { get; private set; }
    public Dictionary<int, GameObject> LocalObjectsDict { get; private set; }

    private List<int> usedGuid;

    public void Init()
    {
        ObjectInfos = new Dictionary<int, ObjectInfo>();
        LocalObjectsDict = new Dictionary<int, GameObject>();
        usedGuid = new();
    }
    
    private void SpawnGathering(int objectId, Vector3 pos, int guid = 0)
    {
        var name = Managers.Data.GatheringDict[objectId].name;
        GameObject go = Managers.Resource.Instantiate($"Gathering/{name}", pos, Quaternion.identity);
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
            if (spawnPoses == null)
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
        var name = Managers.Data.LootingDict[objectId].name;
        GameObject go =  Managers.Resource.Instantiate($"Lootings/{name}", pos, Quaternion.identity);
        LootingItemController lc = go.GetOrAddComponent<LootingItemController>();
        if (guid == 0)
            lc.guid = go.GetInstanceID();
        else
            lc.guid = guid;
        lc.id = objectId;
        ObjectInfos.Add(lc.guid, new ObjectInfo(){objectID=objectId,guid=lc.guid,y = pos.y,x = pos.x});
        LocalObjectsDict.Add(lc.guid, go);
        return lc;
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
    }
    
    public void SpawnLootingItems(){}

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

    public int GenerateGuid()
    {
        int key = 0;
        for (int i = 1; i < 255; i++)
        {
            if (!ObjectInfos.ContainsKey(i) && !usedGuid.Contains(i))
            {
                key = i;
                usedGuid.Add(i);
                break;
            }
        }
        return key;
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
}
