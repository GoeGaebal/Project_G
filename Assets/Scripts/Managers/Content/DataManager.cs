using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public interface IDataPersistence
{
    void SaveData();
    void LoadData();
}
public class DataManager
{
    // public Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();
    public Dictionary<int, Item> ItemDict { get; private set; } = new Dictionary<int, Item>();
    public Dictionary<int, GatheringData> GatheringDict { get; private set; } = new Dictionary<int, GatheringData>();
    public Dictionary<int, WorldmapData> WorldmapDict { get; private set; } = new Dictionary<int, WorldmapData>();

    public Dictionary<int, CraftData> CraftDict { get; private set; } = new Dictionary<int, CraftData>();
    public Dictionary<int, Artifact> ArtifactDict { get; private set; } = new Dictionary<int, Artifact>();

    public void Init()
    {
        GatheringDict = LoadJson<GatheringDataLoader, int, GatheringData>("GatheringData").MakeDict();
        
        // TODO: ItemDict을 json으로 바꾸던지 아니면 ScriptableObject로 바꿀건지 생각해야 할 듯
        //AddItems(new [] {"Apple", "IronIngot", "Pipe", "CPU", "Motherboard", "Transistor", "Antenna", "NanoBlade", "LaserGun", "Gauntlet"});
        ItemDict = LoadJson<ItemDataLoader, int, Item>("ItemData").MakeDict();
        WorldmapDict = LoadJson<WorldmapDataLoader, int, WorldmapData>("WorldmapData").MakeDict();
        CraftDict = LoadJson<CraftDataLoader, int, CraftData>("CraftData").MakeDict();
        Artifact artifact = Managers.Resource.Load<Artifact>("Prefabs/Objects/NonCharacter/Interactable/Artifact/Artifacts/Artifact_0");
        ArtifactDict.Add(artifact.ID, artifact);
        artifact = Managers.Resource.Load<Artifact>("Prefabs/Objects/NonCharacter/Interactable/Artifact/Artifacts/Artifact_1");
        ArtifactDict.Add(artifact.ID, artifact);
        artifact = Managers.Resource.Load<Artifact>("Prefabs/Objects/NonCharacter/Interactable/Artifact/Artifacts/Artifact_2");
        ArtifactDict.Add(artifact.ID, artifact);
        artifact = Managers.Resource.Load<Artifact>("Prefabs/Objects/NonCharacter/Interactable/Artifact/Artifacts/Artifact_3");
        ArtifactDict.Add(artifact.ID, artifact);

    }

    private void AddItems(IEnumerable<string> itemList)
    {
        foreach (var it in itemList)
        {
            var item = Managers.Resource.Load<Item>($"Prefabs/UI/Inventory/Item/{it}");
            ItemDict.Add(item.ID,item);
        }
    }

    Loader LoadJson<Loader, TKey, TValue>(string path) where Loader : ILoader<TKey, TValue>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(Path.Combine("Data", path));
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }

    public Dictionary<ulong, bool> LoadMapData(string path)
    {
        return LoadJson<MapDataLoader, ulong, bool>(Path.Combine("Map", path)).MakeDict();
    }

    public void Save(string data, string fileName)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error occured when trying to save data to file: {fullPath}\n{e}");
        }
    }
    
    public string Load(string fileName)
    {
        string dataToLoad = "";
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(fullPath))
        {
            try
            {
                using (FileStream stream = new(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error occured when trying to load data to file: {fullPath}\n{e}");
            }
        }

        return dataToLoad;

    }
}
