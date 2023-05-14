using System.Collections.Generic;
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
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}", Vector3.zero, Quaternion.identity);
        go.name = "Map";
        Tilemap tmBase = Util.FindChild<Tilemap>(go, "Grass_Tilemap", true);
        var cellBounds = tmBase.cellBounds;
        CurrentMapInfo = new MapInfo(mapId,cellBounds.xMin,cellBounds.xMax,cellBounds.yMin,cellBounds.yMax);
        _currentMapInfo = Managers.Data.LoadMapData(mapName);
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

    private ulong Vector2ulong(Vector3 pos)
    {
        int y = (int) pos.y;
        int x = (int) pos.x;
        return ((ulong)(uint)y << Define.INT_SIZE_IN_BITS) | (uint)x;
    }

    public bool IsCollision(Vector3 pos)
    {
        return _currentMapInfo[Vector2ulong(pos)];
    }
}
