using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
    public int target;
    public int targetAmount;
    public int source;
    public int sourceAmount;
    public int material1;
    public int material1Amount;
    public int material2;
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
    public string Name;
    public string Tooltip;
    public string icon;
    public int MaxCount;
    public float HealAmount;
    public int Hp;
    public int Damage;
    public float AttackSpeed;
    public int RecoveryAmount;
}

public class ItemDataLoader : ILoader<int, Item>
{
    public List<ItemData> items = new();

    public Dictionary<int, Item> MakeDict()
    {
        Dictionary<int, Item> dict = new();
        foreach (ItemData item in items)
        {
            var type = item.id / 1000;
            string iconPath = null;
            switch (type)
            {

                case 1:
                    {
                        var weapon = ScriptableObject.CreateInstance<WeaponItem>();
                        iconPath = Path.Combine("Art/Item/Weapon", item.icon);
                        weapon.Init(id: item.id, name: item.Name, tooltip: item.Tooltip, iconPath: iconPath, hp: item.Hp, damage: item.Damage, attackSpeed: item.AttackSpeed);
                        dict.Add(item.id, weapon);
                        break;
                    }
                case 2:
                    {
                        var armor = ScriptableObject.CreateInstance<ArmorItem>();
                        iconPath = Path.Combine("Art/Item/Armor", item.icon);
                        armor.Init(id: item.id, name: item.Name, tooltip: item.Tooltip, iconPath: iconPath, hp: item.Hp, damage: item.Damage);
                        dict.Add(item.id, armor);
                        break;
                    }
                case 3:
                    {
                        var healthPotion = ScriptableObject.CreateInstance<HealthPotionItem>();
                        iconPath = Path.Combine("Art/Item/HealthPotion", item.icon);
                        healthPotion.Init(id: item.id, name: item.Name, tooltip: item.Tooltip, iconPath: iconPath, maxCount: item.MaxCount);
                        dict.Add(item.id, healthPotion);
                        break;
                    }
                case 4:
                    {
                        var type2 = (item.id % 1000) / 100;
                        if (type2 == 0)
                        {
                            var food = ScriptableObject.CreateInstance<FoodItem>();
                            iconPath = Path.Combine("Art/Item/Food", item.icon);
                            food.Init(id: item.id, name: item.Name, tooltip: item.Tooltip, iconPath: iconPath, maxCount: item.MaxCount, healAmount: item.HealAmount);
                            dict.Add(item.id, food);
                        }
                        else
                        {
                            var resource = ScriptableObject.CreateInstance<ResourceItem>();
                            iconPath = Path.Combine("Art/Item/Resource", item.icon);
                            resource.Init(id: item.id, name: item.Name, tooltip: item.Tooltip, iconPath: iconPath, maxCount: item.MaxCount);
                            dict.Add(item.id, resource);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

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
