using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    /// <summary>
    /// 현재 Grid 즉 맵
    /// </summary>
    public Grid CurrentGrid { get; private set; }
    
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
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}", new Vector3(0,0,0),Quaternion.identity);
        go.name = "Map";
        
        CurrentGrid = go.GetComponent<Grid>();
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
            CurrentGrid = null;
        }
    }
}
