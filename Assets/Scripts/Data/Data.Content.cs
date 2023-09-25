using System;
using System.Collections.Generic;
#region Map
[Serializable]
public class MapData
{
    public ulong key;
    public bool isCollision;
}

[Serializable]
public class MapDataLoader : ILoader<ulong,bool>
{
    public List<MapData> mapdatas = new();
    public Dictionary<ulong, bool> MakeDict()
    {
        var dict = new Dictionary<ulong, bool>();
        foreach (var mapData in mapdatas)
        {
            dict.Add(mapData.key, mapData.isCollision);
        }
        return dict;
    }
}
#endregion
