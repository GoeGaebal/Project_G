using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    private readonly Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
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
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        if (_objects.ContainsKey(info.ObjectId)) return;
        
        if (objectType == GameObjectType.Player)
        {
            var go = Managers.Resource.Instantiate("Objects/Character/Player");
            _objects.Add(info.ObjectId, go);
            go.name = info.Name;
            
            Player p = go.GetComponent<Player>();
            p.Id = info.ObjectId;
            p.Info.PosInfo = info.PosInfo;
            p.SyncPos();
            PlayerDict.Add(p.Id, p);

            if (myPlayer)
            {
                Managers.Network.LocalPlayer = p;
                Managers.Network.LocalPlayer.BindingAction();
            }
            else OtherPlayerDict.Add(p.Id,p);
        }
        else if (objectType == GameObjectType.Monster)
        {
            GameObject go = null;
            _objects.TryGetValue(info.ObjectId, out go);
            if (go == null)
            {
                go = Managers.Resource.Instantiate(info.Name == "BossProto" ? $"Objects/Character/Monster/BossMonster1/BossProto" : $"Objects/Character/Monster/{info.Name}");
            }
            go.name = $"{info.Name}_{info.ObjectId}";
            _objects.Add(info.ObjectId, go);
            var bm = go.GetComponent<BasicMonster>();
            bm.Id = info.ObjectId;
            bm.Info = info;
            bm.SyncPos();
        }
        
        else if (objectType == GameObjectType.Gathering)
        {
            GameObject go = null;
            if (!_objects.TryGetValue(info.ObjectId, out go))
            {
                go = Managers.Resource.Instantiate($"Objects/NonCharacter/Gathering/{info.Name}");
                _objects.TryAdd(info.ObjectId, go);
            }

            go.transform.position = new Vector3(info.PosInfo.PosX, info.PosInfo.PosY);
            GatheringController gc = go.GetOrAddComponent<GatheringController>();
            gc.Id = info.ObjectId;
            gc.PosInfo.PosX = info.PosInfo.PosX;
            gc.PosInfo.PosY = info.PosInfo.PosY;
            gc.SyncPos();
        }
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
        _objects.Clear();
    }
}
