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
    public Dictionary<int, Looting> LootingDict { get; private set; } = new Dictionary<int, Looting>();
    public Dictionary<int, Gathering> GatheringDict { get; private set; } = new Dictionary<int, Gathering>();
    
    public void Init()
    {
        LootingDict = LoadJson<LootingData, int, Looting>("LootingData").MakeDict();
        GatheringDict = LoadJson<GatheringData, int, Gathering>("GatheringData").MakeDict();
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
