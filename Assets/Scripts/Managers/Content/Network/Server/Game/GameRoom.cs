using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using Unity.VisualScripting;
using UnityEngine;

public class GameRoom
{
    public int RoomId { get; set; }
    private static Dictionary<int, Player> _players = new Dictionary<int, Player>();
    public int PlayersCount => _players.Count;

    public void EnterGame(ClientSession session, NetworkObject gameObject)
    {
        if (gameObject == null)
            return;
    
        GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);
    
        if (type == GameObjectType.Player)
        {
            Player player = gameObject.GetComponent<Player>();
            _players.Add(gameObject.Id, player);
            player.Room = this;
    
            // Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y));
    
            // 본인한테 정보 전송
            {
                S_EnterGame enterPacket = new S_EnterGame();
                enterPacket.Player = player.Info;
                session.Send(enterPacket);
    
                S_Spawn spawnPacket = new S_Spawn();
                foreach (Player p in  _players.Values)
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
            foreach (Player p in _players.Values)
            {
                if (p.Id != gameObject.Id)
                    p.Session.Send(spawnPacket);
            }
        }
    }

    public void LoadScene()
    {
        foreach (Player p in  _players.Values)
        {
            S_EnterGame enterPacket = new S_EnterGame();
            enterPacket.Player = p.Info;
            p.Session.Send(enterPacket);
        }
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
            if (_players.Remove(objectId, out player) == false)
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
            foreach (Player p in _players.Values)
            {
                if (p.Id != objectId)
                    p.Session.Send(despawnPacket);
            }
        }
    }

    public Player FindPlayerById(int objectId)
    {
        _players.TryGetValue(objectId, out var ret);
        return ret;
    }
    
    public void Broadcast(IMessage packet)
    {
        foreach (Player p in _players.Values)
        {
            p.Session.Send(packet);
        }
    }
}