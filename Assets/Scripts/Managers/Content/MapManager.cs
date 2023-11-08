using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct MapInfo
{
    public MapInfo(int mapId, int minX, int maxX, int minY, int maxY)
    {
        MapId = mapId;
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }
    public int MapId { get; private set; }
    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }
}

public class MapManager
{
    /// <summary>현재 Grid 즉 맵</summary>
    private Dictionary<ulong, bool> _currentMapInfo;

    public MapInfo CurrentMapInfo { get; private set; }

    public int currentMapId = 0;

    /// <summary>
    /// 프리팹에서 맵을 생성하는 함수
    /// </summary>
    /// <remarks>
    /// 맵은 Prefab의 Map폴더에 위치시켜야 하며 Map_000 형태로 저장된다.
    /// </remarks>>
    /// <param name="mapId">
    /// int형 변수, Map_001을 인스턴트화 하고 싶으면 1을 넘기면 됨
    /// </param>
    public void LoadMap(int mapId)
    {
        DestoryMap();
        // 1인 경우 000 중 마지막 0만 1로 바꿈
        string mapName = "Map_" + mapId.ToString("000");
        currentMapId = mapId;
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}", Vector3.zero, Quaternion.identity);
        go.name = "Map";
        Tilemap tmBase = Util.FindChild<Tilemap>(go, "Grass_Tilemap", true);
        var cellBounds = tmBase.cellBounds;
        CurrentMapInfo = new MapInfo(mapId,cellBounds.xMin,cellBounds.xMax,cellBounds.yMin,cellBounds.yMax);
        _currentMapInfo = Managers.Data.LoadMapData(mapName);
        
        BasicMonster[] monsters = go.GetComponentsInChildren<BasicMonster>();
        if (Managers.Network.IsHost) Managers.Network.Server.Room.SpawnMonsters(monsters);
        foreach (var monster in monsters)
            Managers.Resource.Destroy(monster.gameObject);
        GatheringController[] gatherings = go.GetComponentsInChildren<GatheringController>();
        if (Managers.Network.IsHost) Managers.Network.Server.Room.SpawnGatherings(gatherings);
        foreach (var gathering in gatherings)
            Managers.Resource.Destroy(gathering.gameObject);
    }

    public void LoadMap(string mapName)
    {
        DestoryMap();
        // 1인 경우 000 중 마지막 0만 1로 바꿈
        //string mapName = "Map_" + mapId.ToString("000");
        string temp = mapName.Substring(4,3);
        currentMapId = int.Parse(temp);
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}", Vector3.zero, Quaternion.identity);
        go.name = "Map";
        Tilemap tmBase = Util.FindChild<Tilemap>(go, "Grass_Tilemap", true);
        var cellBounds = tmBase.cellBounds;
        CurrentMapInfo = new MapInfo(currentMapId, cellBounds.xMin, cellBounds.xMax, cellBounds.yMin, cellBounds.yMax);
        _currentMapInfo = Managers.Data.LoadMapData(mapName);

        BasicMonster[] monsters = go.GetComponentsInChildren<BasicMonster>();
        if (Managers.Network.isHost) Managers.Network.Server.Room.SpawnMonsters(monsters);
        foreach (var monster in monsters)
            Managers.Resource.Destroy(monster.gameObject);
        GatheringController[] gatherings = go.GetComponentsInChildren<GatheringController>();
        if (Managers.Network.isHost) Managers.Network.Server.Room.SpawnGatherings(gatherings);
        foreach (var gathering in gatherings)
            Managers.Resource.Destroy(gathering.gameObject);
    }

    // TODO : 충돌 지점이 정중앙이기 때문에 원하는 느낌이 나오지 못함
    public bool CheckCanGo(Vector2 pos)
    {
        bool value;
        int y = (int)pos.y;
        int x = (int)pos.x;
        // 만일 음수면 -0.xx가 -1이 아니라 0이 되버린다. 그걸 방지
        if (pos.y < 0.0f) y--; if (pos.x < 0.0f) x--;
       
        ulong key = ((ulong)(uint)y << Define.INT_SIZE_IN_BITS) | (uint)x;
        if (_currentMapInfo.TryGetValue(key, out value))
            return !value;
        else
            return !false;
    }

    public Vector3 GenerateCurrentRandPos()
    {
        //랜덤 위치 스폰 (일단 겹치더라도 상관없이)
        return new Vector3()
        {
            y = Random.Range(CurrentMapInfo.MinY, CurrentMapInfo.MaxY),
            x = Random.Range(CurrentMapInfo.MinX, CurrentMapInfo.MaxX)
        };
    }
    
    /// <summary>
    /// Hierarchy창에서 Map 오브젝트를 찾아 Destory하는 함수
    /// </summary>
    public void DestoryMap()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            Object.Destroy(map);
            _currentMapInfo = null;
            currentMapId = 0;
        }
    }

    public bool IsCollision(Vector3 pos)
    {
        return _currentMapInfo[Util.Vector2ulong(pos)];
    }
}
