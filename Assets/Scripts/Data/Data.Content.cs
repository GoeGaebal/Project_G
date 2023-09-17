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

#region Craft
[Serializable]
public class CraftData
{
    public int id;
    public string target;
    public int targetAmount;
    public string source;
    public int sourceAmount;
    public string material1;
    public int material1Amount;
    public string material2;
    public int material2Amount;
}

public class CraftDataLoader : ILoader<int, CraftData>
{
    public List<CraftData> crafts = new();

    public Dictionary<int, CraftData> MakeDict()
    {
        Dictionary<int, CraftData> dict = new();
        foreach (CraftData craft in crafts)
            dict.Add(craft.id, craft);
        return dict;
    }
}
#endregion

#region Gathering
[Serializable]
public class GatheringData
{
    public int id;
    public string name;
    public float maxHp;
    public int lootingId;
}

public class GatheringDataLoader : ILoader<int, GatheringData>
{
    public List<GatheringData> gatherings = new();

    public Dictionary<int, GatheringData> MakeDict()
    {
        Dictionary<int, GatheringData> dict = new();
        foreach (GatheringData gathering in gatherings)
            dict.Add(gathering.id, gathering);
        return dict;
    }
}
#endregion

#region Item
[Serializable]
public class ItemData
{
    public int id;
    public string name;
}

public class ItemDataLoader : ILoader<int, ItemData>
{
    public List<ItemData> items = new();

    public Dictionary<int, ItemData> MakeDict()
    {
        Dictionary<int, ItemData> dict = new();
        foreach (ItemData item in items)
            dict.Add(item.id, item);
        return dict;
    }
}
#endregion

#region Worldmap
[Serializable]
public class WorldmapData
{
    public int id;
    public string name;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public string weather;
}

public class WorldmapDataLoader : ILoader<int, WorldmapData>
{
    public List<WorldmapData> worldmaps = new();

    public Dictionary<int, WorldmapData> MakeDict()
    {
        Dictionary<int, WorldmapData> dict = new();
        foreach (WorldmapData worldmap in worldmaps)
            dict.Add(worldmap.id, worldmap);
        return dict;
    }
}
#endregion
