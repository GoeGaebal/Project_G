using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}
public class DataManager
{
    // public Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();
    public Dictionary<int, Looting> LootingDict { get; private set; } = new Dictionary<int, Looting>();
    public Dictionary<int, Gathering> GatheringDict { get; private set; } = new Dictionary<int, Gathering>();
    public Dictionary<int, Worldmap> WorldmapDict { get; private set; } = new Dictionary<int, Worldmap>();

    public void Init()
    {
        LootingDict = LoadJson<LootingData, int, Looting>("LootingData").MakeDict();
        GatheringDict = LoadJson<GatheringData, int, Gathering>("GatheringData").MakeDict();
        WorldmapDict = LoadJson<WorldmapData, int, Worldmap>("WorldmapData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }

    public Dictionary<ulong, bool> LoadMapData(string path)
    {
        return LoadJson<MapDataLoader, ulong, bool>($"Map/{path}").MakeDict();
    }
}
