using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectManager
{
    private readonly Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    private readonly Dictionary<int, BasicMonster> _monsters = new Dictionary<int, BasicMonster>();
    private readonly Dictionary<int, GatheringController> _gatherings = new Dictionary<int, GatheringController>();
    public readonly Dictionary<int, Player> PlayerDict = new Dictionary<int, Player>();
    public readonly Dictionary<int, Player> OtherPlayerDict = new Dictionary<int, Player>();

    public static GameObjectType GetObjectTypeById(int id)
    {
        var type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        if (_objects.ContainsKey(info.ObjectId)) return;
        
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        GameObject go = null;
        switch (objectType)
        { 
            case GameObjectType.Player:
            {
                go = Managers.Resource.Instantiate("Objects/Character/Player");
                go.name = $"Player_{info.ObjectId}";

                Player p = go.GetComponent<Player>();
                p.Id = info.ObjectId;
                p.Info.PosInfo = info.PosInfo;
                p.SyncPos();
                PlayerDict.Add(p.Id, p);

                if (myPlayer)
                {
                    Managers.Network.LocalPlayer = p;
                    p.Info.Name = Managers.Network.UserName;
                    p.Name = p.Info.Name;
                    Managers.Network.Client.Send(new C_ChangeName() { Name = p.Info.Name
                    });
                    Managers.Network.LocalPlayer.BindingAction();
                }
                else
                {
                    OtherPlayerDict.Add(p.Id,p);
                    p.Name = info.Name;
                }
                break;
            }
            case GameObjectType.Monster:
            {
                go = Managers.Resource.Instantiate(info.Name == "GasBoss" ? $"Objects/Character/Monster/BossMonster1/GasBoss" : $"Objects/Character/Monster/{info.Name}");
                if(info.Name == "GasBoss")
                {
                    go.transform.localScale = new Vector3(2.5f, 2.5f, 1);
                }
                go.name = $"{info.Name}_{info.ObjectId}";
                var bm = go.GetComponent<BasicMonster>();
                bm.Id = info.ObjectId;
                bm.Info = info;
                bm.SyncPos();
                _monsters.Add(bm.Id, bm);
                break;
            }
            case GameObjectType.Gathering:
            {
                go = Managers.Resource.Instantiate($"Objects/NonCharacter/Gathering/{info.Name}");

                go.transform.position = new Vector3(info.PosInfo.PosX, info.PosInfo.PosY);
                GatheringController gc = go.GetOrAddComponent<GatheringController>();
                gc.Id = info.ObjectId;
                gc.PosInfo.PosX = info.PosInfo.PosX;
                gc.PosInfo.PosY = info.PosInfo.PosY;
                gc.SyncPos();
                _gatherings.Add(gc.Id, gc);
                break;
            }
            case GameObjectType.None:
            case GameObjectType.LootingItem:
            default:
                break;
        }
        Object.DontDestroyOnLoad(go);
        _objects.Add(info.ObjectId, go);
    }
    public void Add(LootingInfo info)
    {
        GameObject go = null;
        if (!_objects.TryGetValue(info.ObjectId, out go))
        {
            go = Managers.Resource.Instantiate($"Objects/NonCharacter/Lootings/apple");
            _objects.TryAdd(info.ObjectId, go);
        }

        go.transform.position = new Vector3(info.PosX, info.PosY);
        LootingItemController lc = go.GetOrAddComponent<LootingItemController>();
        lc.Id = info.ObjectId;
        var item = Managers.Data.ItemDict[info.LootingId];
        lc.Item = item;
        lc.Bounce(new Vector3(info.DestPosX, info.DestPosY));
    }

    public void Remove(int id)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;
        
        GameObjectType objectType = GetObjectTypeById(id);
        switch (objectType)
        {
            case GameObjectType.Monster:
                _monsters.Remove(id);
                break;
            case GameObjectType.Gathering:
                _gatherings.Remove(id);
                break;
            case GameObjectType.Player:
                PlayerDict.Remove(id);
                OtherPlayerDict.Remove(id);
                break;
            case GameObjectType.None:
            case GameObjectType.LootingItem:
            default:
                break;
        }

        _objects.Remove(id);	// 딕셔너리에서 삭제
        // 실질적으로 게임화면에서 삭제
        Managers.Resource.Destroy(go);
    }
    
    public void RemoveMyPlayer()
    {
        GameObject go = Managers.Network.LocalPlayer.gameObject;
        if (go == null)
            return;

        _objects.Remove(go.GetComponent<Player>().Id);
        Managers.Resource.Destroy(go);
    }

    public void Clear()
    {
        foreach (var obj in _objects.Values.Where(obj => obj != null))
        {
            Managers.Resource.Destroy(obj);
        }
        PlayerDict.Clear();
        OtherPlayerDict.Clear();
        _monsters.Clear();
        _gatherings.Clear();
        _objects.Clear();
    }

    public void ClearObjects(params GameObjectType[] types)
    {
        foreach (var type in types)
        {
            if (type == GameObjectType.Monster)
            {
                foreach (var key in _monsters.Keys)
                {
                    Managers.Resource.Destroy(_objects[key]);
                    _objects.Remove(key);
                }
                _monsters.Clear();
            }
            else if (type == GameObjectType.Gathering)
            {
                foreach (var key in _gatherings.Keys)
                {
                    Managers.Resource.Destroy(_objects[key]);
                    _objects.Remove(key);
                }
                _gatherings.Clear();
            }
            else if (type == GameObjectType.Player)
            {
                foreach (var key in PlayerDict.Keys)
                {
                    Managers.Resource.Destroy(_objects[key]);
                    _objects.Remove(key);
                }
                PlayerDict.Clear();
                OtherPlayerDict.Clear();
            }
        }
    }
}
