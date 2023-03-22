using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public Grid CurrentGrid { get; private set; }
    public void LoadMap(int mapId)
    {
        DestoryMap();
        // 1인 경우 000 중 마지막 0만 1로 바꿈
        string mapName = "Map_" + mapId.ToString("000");
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";
        
        CurrentGrid = go.GetComponent<Grid>();
    }
    
    public void DestoryMap()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }
}
