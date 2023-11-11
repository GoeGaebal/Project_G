using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using UnityEngine;

public class GameRoom
{
    public int RoomId { get; set; }
    public readonly Dictionary<int, ClientSession> PlayersSessions = new Dictionary<int, ClientSession>();
    private readonly Dictionary<int, ObjectInfo> _players = new Dictionary<int, ObjectInfo>();
    private readonly Dictionary<int, ObjectInfo> _objects = new Dictionary<int, ObjectInfo>();
    public int PlayersCount => _players.Count;
    
    private static int _counter = 0;
    private static int _monsterCounter = 0;
    private static int _lootingCounter = 0;
    private static int _gatheringCounter = 0;
    
    public static int GenerateId(GameObjectType type)
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
    
    private ObjectInfo FindById(int id)
    {
        ObjectInfo go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public void EnterGame(ClientSession session, ObjectInfo gameObject)
    {
        if (gameObject == null)
            return;
    
        GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.ObjectId);
    
        if (type == GameObjectType.Player)
        {
            // TODO : 들어왔을 때 정보 초기화
            gameObject.PosInfo = new PositionInfo()
            {
                PosX = 0.0f, PosY = 0.0f, Dir = 1,
            };
            
            _players.Add(gameObject.ObjectId, gameObject);
            PlayersSessions.Add(gameObject.ObjectId, session);
            // 본인한테 정보 전송
            {
                S_EnterGame enterPacket = new S_EnterGame();
                enterPacket.Player = gameObject;
                session.Send(enterPacket);
    
                var spawnPacket = new S_Spawn();
                foreach (var p in  _players.Values)
                {
                    if (gameObject.ObjectId != p.ObjectId)  spawnPacket.Objects.Add(p);
                }
    
                foreach (var m in _objects.Values)
                    spawnPacket.Objects.Add(m);
                //
                // foreach (Projectile p in _projectiles.Values)
                //     spawnPacket.Objects.Add(p.Info);
    
                session.Send(spawnPacket);
            }
        }
        // else if (type == GameObjectType.Monster)
        // {
        //     Monster monster = gameObject as Monster;
        //     _monsters.Add(gameObject.Id, monster);
        //     monster.Room = this;
        //
        //     Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));
        // }
        // else if (type == GameObjectType.Projectile)
        // {
        //     Projectile projectile = gameObject as Projectile;
        //     _projectiles.Add(gameObject.Id, projectile);
        //     projectile.Room = this;
        // }
			 
        // 타인한테 정보 전송
        {
            S_Spawn spawnPacket = new S_Spawn();
            spawnPacket.Objects.Add(gameObject);
            foreach (var p in _players.Values)
            {
                if (p.ObjectId != gameObject.ObjectId)
                    PlayersSessions[p.ObjectId].Send(spawnPacket);
            }
        }
    }

    public void LoadScene(SceneType type)
    {
        S_LoadScene packet = new S_LoadScene();
        packet.SceneType = type;
        _objects.Clear();
        Managers.Network.Server.Room.Broadcast(packet);
    }

    public void HandleMove(Player player, C_PlayerMove movePacket)
    {
        if (player == null) return;

        // TODO : 검증
        var movePosInfo = movePacket.PosInfo;
        var info = player.Info;
        if (!_players.TryGetValue(info.ObjectId, out var p)) return;
        
        p.PosInfo = movePosInfo.PosInfo;
        var resMovePacket = new S_PlayerMove
        {
            ObjectId = player.Info.ObjectId,
            PosInfo = movePosInfo
        };

        Broadcast(player.Id, resMovePacket);
    }
    
    public void LeaveGame(int objectId)
    {
        GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

        if (type == GameObjectType.Player)
        {
            if (_players.Remove(objectId, out var player) == false)
                return;

            // Map.ApplyLeave(player);
            // player.Room = null;

            // 본인한테 정보 전송
            {
                S_LeaveGame leavePacket = new S_LeaveGame();
                PlayersSessions[player.ObjectId].Send(leavePacket);
                PlayersSessions.Remove(player.ObjectId);
            }
        }
        // else if (type == GameObjectType.Monster)
        // {
        //     Monster monster = null;
        //     if (_monsters.Remove(objectId, out monster) == false)
        //         return;
        //
        //     monster.Room = null;
        //     Map.ApplyLeave(monster);
        // }
        // else if (type == GameObjectType.Projectile)
        // {
        //     Projectile projectile = null;
        //     if (_projectiles.Remove(objectId, out projectile) == false)
        //         return;
        //
        //     projectile.Room = null;
        // }

        // 타인한테 정보 전송
        {
            S_DeSpawn despawnPacket = new S_DeSpawn();
            despawnPacket.ObjectIds.Add(objectId);
            foreach (var p in _players.Values)
            {
                PlayersSessions[p.ObjectId].Send(despawnPacket);
            }
        }
        
        // TODO: 서버 종료
        if (_players.Count == 0) Clear();
    }

    public void SpawnMonsters(BasicMonster[] monsters)
    {
        S_Spawn spawn = new S_Spawn();
        foreach (var monster in monsters)
        {
            monster.Id = GenerateId(GameObjectType.Monster);
            string name = monster.gameObject.name;
            int end = name.IndexOf('(');
            if (end >= 0)
                name = name.Substring(0, end).Trim();
            
            monster.Info.Name = name;
            var position = monster.transform.position;
            monster.PosInfo.PosX = position.x;
            monster.PosInfo.PosY = position.y;
            monster.PosInfo.State = CreatureState.Idle;
            spawn.Objects.Add(monster.Info);
            _objects.Add(monster.Id, monster.Info);
        }
        Broadcast(spawn);
    }
    
    public void SpawnGatherings(GatheringController[] gatherings)
    {
        S_Spawn spawn = new S_Spawn();
        foreach (var gathering in gatherings)
        {
            gathering.Id = GenerateId(GameObjectType.Gathering);
            string name = gathering.gameObject.name;
            int end = name.IndexOf('(');
            if (end >= 0)
                name = name.Substring(0, end).Trim();
            
            gathering.Info.Name = name;
            var position = gathering.transform.position;
            gathering.PosInfo.PosX = position.x;
            gathering.PosInfo.PosY = position.y;
            gathering.PosInfo.State = CreatureState.Idle;
            spawn.Objects.Add(gathering.Info);
            _objects.Add(gathering.Id,gathering.Info);
        }
        Broadcast(spawn);
    }
    
    public void SpawnLootingItems(int objectId,int count, Vector3 pos, float maxRadious = 10.0f,float minRadious = 0.0f)
    {
        S_SpawnLooting spawn = new S_SpawnLooting();
        for (int i = 0; i < count; i++)
        {
            
            Vector2 randPos = Random.insideUnitCircle * Random.Range(minRadious,maxRadious);
            randPos.x += pos.x;
            randPos.y += pos.y;
            LootingInfo info = new()
            {
                ObjectId = GenerateId(GameObjectType.LootingItem),
                LootingId = objectId,
                PosX = pos.x,
                PosY = pos.y,
                DestPosX = randPos.x,
                DestPosY = randPos.y
            };
            spawn.Infos.Add(info);
        }
        Broadcast(spawn);
    }

    public ObjectInfo FindPlayerById(int objectId)
    {
        _players.TryGetValue(objectId, out var ret);
        return ret;
    }
    
    public void Broadcast(IMessage packet)
    {
        foreach (var p in _players.Values)
        {
            PlayersSessions[p.ObjectId].Send(packet);
        }
    }
    
    /// <summary>
    /// 모든 플레이어에게 패킷을 보내는 함수
    /// <remarks> isHost가 true일때만 사용할 것</remarks>>
    /// </summary>
    /// <param name="exceptId">특정 플레이어 아이디만 제외하고 보낸다.</param>
    /// <param name="packet">보내는 패킷</param>
    public void Broadcast(int exceptId, IMessage packet)
    {
        foreach (var p in _players.Values.Where(p => p.ObjectId != exceptId))
        {
            PlayersSessions[p.ObjectId].Send(packet);
        }
    }

    public void Clear()
    {
        _objects.Clear();
        _players.Clear();
        _counter = _gatheringCounter = _lootingCounter = _monsterCounter = 0;
        Managers.Network.Server.ShutDown();
    }
}