using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using Unity.VisualScripting;
using UnityEngine;
using ObjectInfo = Google.Protobuf.Protocol.ObjectInfo;

public class GameRoom
{
    public int RoomId { get; set; }
    public Dictionary<int, Player> Players = new Dictionary<int, Player>();
    public int PlayersCount => Players.Count;

    public void EnterGame(ClientSession session, NetworkObject gameObject)
    {
        if (gameObject == null)
            return;
    
        GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);
    
        if (type == GameObjectType.Player)
        {
            Player player = gameObject.GetComponent<Player>();
            Players.Add(gameObject.Id, player);
            player.Room = this;
    
            // Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y));
    
            // 본인한테 정보 전송
            {
                S_EnterGame enterPacket = new S_EnterGame();
                enterPacket.Player = player.Info;
                session.Send(enterPacket);
    
                S_Spawn spawnPacket = new S_Spawn();
                foreach (Player p in  Players.Values)
                {
                    if (player.Id != p.Id)  spawnPacket.Objects.Add(p.Info);
                }
    
                // foreach (Monster m in _monsters.Values)
                //     spawnPacket.Objects.Add(m.Info);
                //
                // foreach (Projectile p in _projectiles.Values)
                //     spawnPacket.Objects.Add(p.Info);
    
                player.Session.Send(spawnPacket);
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
            spawnPacket.Objects.Add(gameObject.Info);
            foreach (Player p in Players.Values)
            {
                if (p.Id != gameObject.Id)
                    p.Session.Send(spawnPacket);
            }
        }
    }

    public void LoadScene(SceneType type)
    {
        S_LoadScene packet = new S_LoadScene();
        packet.SceneType = type;
        Managers.Network.Server.Room.Broadcast(packet);
    }

    public void HandleMove(Player player, C_Move movePacket)
    {
        if (player == null)
            return;

        // // TODO : 검증
        PositionInfo movePosInfo = movePacket.PosInfo;
        var info = player.Info;
        //
        // // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
        // if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
        // {
        //     if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
        //         return;
        // }

        // info.PosInfo.State = movePosInfo.State;
        // info.PosInfo.MoveDir = movePosInfo.MoveDir;
        // Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

        // 다른 플레이어한테도 알려준다
        S_Move resMovePacket = new S_Move();
        resMovePacket.ObjectId = player.Info.ObjectId;
        resMovePacket.PosInfo = movePacket.PosInfo;

        Broadcast(resMovePacket);
    }
    
    public void LeaveGame(int objectId)
    {
        GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

        if (type == GameObjectType.Player)
        {
            Player player = null;
            if (Players.Remove(objectId, out player) == false)
                return;

            // Map.ApplyLeave(player);
            player.Room = null;

            // 본인한테 정보 전송
            {
                S_LeaveGame leavePacket = new S_LeaveGame();
                player.Session.Send(leavePacket);
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
            S_Despawn despawnPacket = new S_Despawn();
            despawnPacket.ObjectIds.Add(objectId);
            foreach (Player p in Players.Values)
            {
                if (p.Id != objectId) p.Session.Send(despawnPacket);
            }
        }
    }

    public void SpawnMonsters(BasicMonster[] monsters)
    {
        S_Spawn spawn = new S_Spawn();
        foreach (var monster in monsters)
        {
            monster.Id = Managers.Object.GenerateId(GameObjectType.Monster);
            string name = monster.gameObject.name;
            int end = name.IndexOf('(');
            if (end >= 0)
                name = name.Substring(0, end).Trim();
            
            monster.Info.Name = name;
            monster.PosInfo.PosX = monster.transform.position.x;
            monster.PosInfo.PosY = monster.transform.position.y;
            monster.PosInfo.State = CreatureState.Idle;
            spawn.Objects.Add(monster.Info);
        }
        Broadcast(spawn);
    }
    
    public void SpawnGatherings(GatheringController[] gatherings)
    {
        S_Spawn spawn = new S_Spawn();
        foreach (var gathering in gatherings)
        {
            gathering.Id = Managers.Object.GenerateId(GameObjectType.Gathering);
            string name = gathering.gameObject.name;
            int end = name.IndexOf('(');
            if (end >= 0)
                name = name.Substring(0, end).Trim();
            
            gathering.Info.Name = name;
            gathering.PosInfo.PosX = gathering.transform.position.x;
            gathering.PosInfo.PosY = gathering.transform.position.y;
            gathering.PosInfo.State = CreatureState.Idle;
            spawn.Objects.Add(gathering.Info);
        }
        Broadcast(spawn);
    }
    
    public void SpawnLootingItems(int objectId,int count, Vector3 pos, float maxRadious = 10.0f,float minRadious = 0.0f)
    {
        S_Spawn spawn = new S_Spawn();
        for (int i = 0; i < count; i++)
        {
            Google.Protobuf.Protocol.ObjectInfo info = new();
            Vector2 randPos = Random.insideUnitCircle * Random.Range(minRadious,maxRadious);
            randPos.x += pos.x;
            randPos.y += pos.y;
            info.ObjectId = Managers.Object.GenerateId(GameObjectType.LootingItem);
            info.PosInfo = new PositionInfo();
            info.PosInfo.PosX = pos.x;
            info.PosInfo.PosY = pos.y;
            info.PosInfo.WposX = randPos.x;
            info.PosInfo.WposY = randPos.y;
            info.PosInfo.Dir = objectId;
            spawn.Objects.Add(info);
        }
        Broadcast(spawn);
    }

    public Player FindPlayerById(int objectId)
    {
        Players.TryGetValue(objectId, out var ret);
        return ret;
    }
    
    public void Broadcast(IMessage packet)
    {
        foreach (Player p in Players.Values)
        {
            p.Session.Send(packet);
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
        foreach (Player p in Players.Values)
        {
            if(p.Id != exceptId) p.Session.Send(packet);
        }
    }
}