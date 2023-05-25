using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

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

    public void Init()
    {
        ObjectInfos = new Dictionary<int, ObjectInfo>();
        LocalObjectsDict = new Dictionary<int, GameObject>();
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
                    ObjectInfos.Add(guid, new ObjectInfo(){objectID=objectId,guid=guid,y = randPos.y,x = randPos.x});
                    LocalObjectsDict.Add(guid,go);
                }
            }
            else
            {
                foreach (Vector3 spawnPos in spawnPoses)
                {
                    GameObject go = SpawnGathering(objectId, spawnPos);
                    ObjectInfos.Add(go.GetInstanceID(),new ObjectInfo(){objectID=objectId,guid=go.GetInstanceID(),y = spawnPos.y,x = spawnPos.x});
                    LocalObjectsDict.Add(go.GetInstanceID(),go);
                }
                for (int i = 0; i < count - spawnPoses.Length; i++)
                {
                    Vector3 randPos = Managers.Map.GenerateCurrentRandPos();
                    GameObject go = SpawnGathering(objectId,randPos);
                    ObjectInfos.Add(go.GetInstanceID(),new ObjectInfo(){objectID=objectId,guid=go.GetInstanceID(),y = randPos.y,x = randPos.x});
                    LocalObjectsDict.Add(go.GetInstanceID(),go);
                }
            }
        }
        else
        {
            Managers.Network.RequestGatherings();
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
            var go = SpawnGathering(infos[toInstantiateKey].objectID, new Vector3(infos[toInstantiateKey].x,infos[toInstantiateKey].y) );
            LocalObjectsDict.Add(toInstantiateKey,go);
        }
    }
}
