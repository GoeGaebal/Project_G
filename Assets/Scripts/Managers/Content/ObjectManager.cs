using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectManager
{
    public class ObjectInfo
    {
        public int objectId;
        public int y;
        public int x;
    }
    
    private Dictionary<int, ObjectInfo> _objects = new Dictionary<int, ObjectInfo>();
    
    public void SpawnGatherings(int count)
    {
        var minX = Managers.Map.CurrentMapInfo.MinX;
        var maxX = Managers.Map.CurrentMapInfo.MaxX;
        var minY = Managers.Map.CurrentMapInfo.MinY;
        var maxY = Managers.Map.CurrentMapInfo.MaxY;
        for (int i = 0; i < count; i++)
        {
            //랜덤 위치 스폰 (일단 겹치더라도 상관없이)
            Vector3Int pos = new Vector3Int()
            {
                y = Random.Range(minY, maxY),
                x = Random.Range(minX, maxX)
            };

            Managers.Resource.Instantiate("Gathering/Stone", pos, Quaternion.identity);
            _objects.Add(i,new ObjectInfo(){objectId = 0, y = pos.y,x = pos.x});
        }
    }

    /// <summary>
    /// 아이템을 뿌려주는 메소드
    /// </summary>
    /// <param name="count">아이템을 촘 몇개 뿌릴 것인지</param>
    /// <param name="spawnPos">뿌려지는 시작 장소</param>
    /// <param name="maxRadious">최대 거리</param>
    /// <param name="minRadious">최소 거리</param>
    public void SpawnLootings(int count, Vector3 spawnPos, float maxRadious = 10.0f,float minRadious = 0.0f)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randPos = Random.insideUnitCircle * Random.Range(minRadious,maxRadious);
            randPos.x += spawnPos.x;
            randPos.y += spawnPos.y;
            GameObject go =  Managers.Resource.Instantiate("Gathering/Coin", spawnPos, Quaternion.identity);
            // TODO : GetComponent라는 비싼 연산을 수행하며 시간과 거리가 하드코딩 되어있음.
            // 아마 오브젝트 풀링을 적용하면 조금은 해결될 것
            LootingItemController lc = go.GetComponent<LootingItemController>();
            lc.Bounce(randPos, 1.0f, 1.0f);
        }
    }
    
    // 서버측
    [PunRPC]
    void SendDictionaryToClients(PhotonMessageInfo info)
    {
        // Send the dictionary to all clients
        Managers.PhotonView.RPC("ReceiveDictionaryFromHost", RpcTarget.AllBuffered, _objects);
    }
    
    // 클라측
    [PunRPC]
    void ReceiveDictionaryFromHost(Dictionary<int, ObjectInfo> dict)
    {
        // Move the received dictionary into your own dictionary
        foreach (KeyValuePair<int, ObjectInfo> pair in dict)
        {
            _objects[pair.Key] = pair.Value;
            Vector3Int pos = new Vector3Int( pair.Value.x,  pair.Value.y, 0);
            Managers.Resource.Instantiate("Gathering/Stone", pos, Quaternion.identity);
        }
    }
}
