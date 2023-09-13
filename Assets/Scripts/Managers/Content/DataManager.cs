using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public Dictionary<int, Gathering> GatheringDict { get; private set; } = new Dictionary<int, Gathering>();
    public Dictionary<int, Worldmap> WorldmapDict { get; private set; } = new Dictionary<int, Worldmap>();
    public Dictionary<int, Craft> CraftDict { get; private set; } = new Dictionary<int, Craft>();

    public void Init()
    {
        GatheringDict = LoadJson<GatheringData, int, Gathering>("GatheringData").MakeDict();
        
        // TODO: ItemDict을 json으로 바꾸던지 아니면 ScriptableObject로 바꿀건지 생각해야 할 듯
        Item item = null;
        item = Managers.Resource.Load<Item>("prefabs/UI/Inventory/Item/Apple");
        ItemDict.Add(item.ID,item);
        item = Managers.Resource.Load<Item>("prefabs/UI/Inventory/Item/IronIngot");
        ItemDict.Add(item.ID,item);
        item = Managers.Resource.Load<Item>("prefabs/UI/Inventory/Item/Sword");
        ItemDict.Add(item.ID,item);

        WorldmapDict = LoadJson<WorldmapData, int, Worldmap>("WorldmapData").MakeDict();

        CraftDict = LoadJson<CraftData, int, Craft>("CraftData").MakeDict();

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
