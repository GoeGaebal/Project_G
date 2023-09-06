using System;
using System.Collections.Generic;


#region Stat
[Serializable]
public class Stat
{
    public int level;
    public int hp;
    public int attack;
}

[Serializable]
public class StatData : ILoader<int,Stat>
{
    public List<Stat> stats = new List<Stat>();
    public Dictionary<int, Stat> MakeDict()
    {
        Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
        foreach(Stat stat in stats)
            dict.Add(stat.level,stat);
        return dict;
    }
}
#endregion

#region Gathering
[Serializable]
public class Gathering
{
    public int id;
    public string name;
    public float maxHp;
    public int lootingId;
}

[Serializable]
public class GatheringData : ILoader<int,Gathering>
{
    public List<Gathering> gatherings = new List<Gathering>();
    public Dictionary<int, Gathering> MakeDict()
    {
        Dictionary<int, Gathering> dict = new Dictionary<int, Gathering>();
        foreach(Gathering gathering in gatherings)
            dict.Add(gathering.id,gathering);
        return dict;
    }
}
#endregion

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

#region Worldmap
[Serializable]
public class Worldmap
{
    public int id;
    public string name;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public string weather;
}

[Serializable]
public class WorldmapData : ILoader<int, Worldmap>
{
    public List<Worldmap> worldmaps = new();

    public Dictionary<int, Worldmap> MakeDict()
    {
        Dictionary<int, Worldmap> dict = new();
        foreach (Worldmap worldmap in worldmaps)
            dict.Add(worldmap.id, worldmap);
        return dict;
    }
}
#endregion

#region SaveData
[Serializable]
public class SaveData
{
    public List<ulong> InventoryList;
}
#endregion

#region
[Serializable]
public class Craft
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

[Serializable]
public class CraftData: ILoader<int, Craft>
{
    public List<Craft> crafts = new();

    public Dictionary<int, Craft> MakeDict()
    {
        Dictionary<int, Craft> dict = new();
        foreach (Craft craft in crafts)
            dict.Add(craft.id, craft);
        return dict;
    }
}
#endregion