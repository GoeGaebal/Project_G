using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.Tilemaps;

public class ObjectManager
{
    public void GenerateGathering(int mapId)
=======
using Random = UnityEngine.Random;

public class ObjectManager
{
    [System.Serializable]
    public class ObjectInfo
    {
        public int objectId;
        public int y;
        public int x;
    }

    public Dictionary<int, ObjectInfo> ObjectsDict { get; private set; }

    public void Init()
    {
        ObjectsDict = new Dictionary<int, ObjectInfo>();
    }

<<<<<<< HEAD
    public void SpawnGatherings(int count)
>>>>>>> bfdaf0d (feat: Lobby System 구축)
=======
    public void SpawnGatherings(int objectId, int count)
>>>>>>> aa2e552 (feat: Implemnet GatheringController)
    {
        string mapName = "Map_" + mapId.ToString("000");
        GameObject go = Resources.Load<GameObject>($"Prefabs/Map/{mapName}");
        if (go == null)
        {
            Debug.Log("Map을 불러오지 못했습니다.");
            return;
        }

        Tilemap tmBase = Util.FindChild<Tilemap>(go, "Grass_Tilemap", true);

        var cellBounds = tmBase.cellBounds;
        int xMin = cellBounds.xMin;
        int xMax = cellBounds.xMax;
        int yMin = cellBounds.yMin;
        int yMax = cellBounds.yMax;

        for (int i = 0; i < 5; i++)
        {
            //랜덤 위치 스폰 (일단 겹치더라도 상관없이)
            Vector3Int pos = new Vector3Int()
            {
<<<<<<< HEAD
                x = Random.Range(xMin, xMax),
                y = Random.Range(yMin, yMax)
            };

            Managers.Resource.Instantiate("Gathering/Bush", pos, Quaternion.identity);
=======
                y = Random.Range(minY, maxY),
                x = Random.Range(minX, maxX)
            };

            var name = Managers.Data.GatheringDict[objectId].name;
            if (name == null)
            {
                Debug.LogError("아이디로부터 채집물 이름을 불러오지 못하였습니다.");
                return;
            }
            Managers.Resource.Instantiate($"Gathering/{name}", pos, Quaternion.identity);
            var id = Managers.Data.GatheringDict[objectId].id;
            ObjectsDict.Add(i,new ObjectInfo(){objectId = id, y = pos.y,x = pos.x});
        }
    }

    /// <summary>
    /// 아이템을 뿌려주는 메소드
    /// </summary>
    /// <param name="count">아이템을 촘 몇개 뿌릴 것인지</param>
    /// <param name="spawnPos">뿌려지는 시작 장소</param>
    /// <param name="maxRadious">최대 거리</param>
    /// <param name="minRadious">최소 거리</param>
    public void SpawnLootings(int objectId,int count, Vector3 spawnPos, float maxRadious = 10.0f,float minRadious = 0.0f)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randPos = Random.insideUnitCircle * Random.Range(minRadious,maxRadious);
            randPos.x += spawnPos.x;
            randPos.y += spawnPos.y;
            var name = Managers.Data.LootingDict[objectId].name;
            if (name == null)
            {
                Debug.LogError("아이디로부터 루팅 아이템 이름을 불러오지 못하였습니다.");
                return;
            }
            GameObject go =  Managers.Resource.Instantiate($"Lootings/{name}", spawnPos, Quaternion.identity);
            // TODO : GetComponent라는 비싼 연산을 수행하며 시간과 거리가 하드코딩 되어있음.
            // 아마 오브젝트 풀링을 적용하면 조금은 해결될 수도
            LootingItemController lc = go.GetComponent<LootingItemController>();
<<<<<<< HEAD
            lc.Bounce(randPos, 1.0f, 1.0f);
>>>>>>> bfdaf0d (feat: Lobby System 구축)
=======
            lc?.Bounce(randPos, 1.0f, 1.0f);
>>>>>>> aa2e552 (feat: Implemnet GatheringController)
        }
    }

    public void SpawningList(Dictionary<int, ObjectInfo> objectInfos)
    {
        foreach (KeyValuePair<int,ObjectInfo> info in objectInfos)
        {
            ObjectsDict[info.Key] = info.Value;
            var name = Managers.Data.LootingDict[info.Value.objectId].name;
            if (name == null)
            {
                Debug.LogError("아이디로부터 루팅 아이템 이름을 불러오지 못하였습니다.");
                return;
            }
            Vector3Int pos = new Vector3Int( info.Value.x,  info.Value.y, 0);
            Managers.Resource.Instantiate($"Gathering/{name}", pos, Quaternion.identity);
        }
    }
}
