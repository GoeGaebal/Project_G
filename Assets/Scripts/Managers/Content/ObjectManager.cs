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

    private int _counter = 0;
    private int _monsterCounter = 0;
    private int _lootingCounter = 0;
    private int _gatheringCounter = 0;
    
    public int GenerateId(GameObjectType type)
    {
        return type switch
        {
            GameObjectType.Player => ((int)type << 24) | (_counter++),
            GameObjectType.Monster => ((int)type << 24) | (_monsterCounter++),
            GameObjectType.LootingItem => ((int)type << 24) | (_lootingCounter++),
            GameObjectType.Gathering => ((int)type << 24) | (_gatheringCounter++),
            _ => 0
        };
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
        if (objectType == GameObjectType.Player)
        {
            if (myPlayer)
            {
                var go = Managers.Resource.Instantiate("Objects/Character/Player");
                Managers.MakeDontDestroyOnLoad(go);
                Managers.Network.LocalPlayer = go.GetComponent<Player>();
                
                _objects.TryAdd(info.ObjectId, Managers.Network.LocalPlayer.gameObject);
                Managers.Network.LocalPlayer.BindingAction();
                
                Managers.Network.LocalPlayer.gameObject.name = info.Name;
                Managers.Network.LocalPlayer.Id = info.ObjectId;
                Managers.Network.LocalPlayer.Info.PosInfo = info.PosInfo;
                Managers.Network.LocalPlayer.SyncPos();
                // LocalPlayer.Stat = info.StatInfo;
                PlayerDict.Add(Managers.Network.LocalPlayer.Id, Managers.Network.LocalPlayer);
            }
            else
            {
                Player p = Managers.Resource.Instantiate("Objects/Character/Player").GetComponent<Player>();
                UnityEngine.Object.DontDestroyOnLoad(p.gameObject);
                
                p.gameObject.name = info.Name;
                _objects.Add(info.ObjectId, p.gameObject);

                p.Id = info.ObjectId;
                p.Info.PosInfo = info.PosInfo;
                p.SyncPos();
                PlayerDict.Add(p.Id, p);
                OtherPlayerDict.Add(p.Id,p);
            }
        }
        else if (objectType == GameObjectType.Monster)
        {
            GameObject go = null;
            _objects.TryGetValue(info.ObjectId, out go);
            if (go == null)
            {
                if (info.Name == "BossProto")
                    go = Managers.Resource.Instantiate($"Objects/Character/Monster/BossMonster1/BossProto");
                else
                    go = Managers.Resource.Instantiate($"Objects/Character/Monster/{info.Name}");
            }
           
            UnityEngine.Object.DontDestroyOnLoad(go);
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
        var objects = _objects.Values.ToArray();
        foreach(var obj in objects) Managers.Resource.Destroy(obj);
        _objects.Clear();
    }
}
