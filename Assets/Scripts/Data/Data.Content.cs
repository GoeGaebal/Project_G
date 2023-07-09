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

#region Lootings
[Serializable]
public class Looting
{
    public int id;
    public string name;
    public float cof;
    public int bounceCount;
    public float threshold;
    public float Sn;
}

[Serializable]
public class LootingData : ILoader<int,Looting>
{
    public List<Looting> lootings = new List<Looting>();
    public Dictionary<int, Looting> MakeDict()
    {
        var dict = new Dictionary<int, Looting>();
        foreach (var looting in lootings)
        {
            dict.Add(looting.id, looting);
        }
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

#region SaveData
[Serializable]
public class SaveData
{
    public List<ulong> InventoryList;
}
#endregion