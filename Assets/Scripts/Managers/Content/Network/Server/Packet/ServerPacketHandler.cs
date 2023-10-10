﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

partial class PacketHandler
{
	public static void C_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		C_LeaveGame leavePacket = packet as C_LeaveGame;
		ClientSession clientSession = session as ClientSession;
		
		Player player = clientSession.MyPlayer;
		if (player == null)  return;

		GameRoom room = player.Room;
		if (room == null) return;

		room.HandleLeave(player, leavePacket);
	}
	
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;
		
		Player player = clientSession.MyPlayer;
		if (player == null)  return;

		GameRoom room = player.Room;
		if (room == null) return;

		room.HandleMove(player,movePacket);
	}

	public static void C_SkillHandler(PacketSession session, IMessage packet)
	{
		// C_Skill skillPacket = packet as C_Skill;
		// ClientSession clientSession = session as ClientSession;
		//
		// Player player = clientSession.MyPlayer;
		// if (player == null)
		// 	return;
		//
		// GameRoom room = player.Room;
		// if (room == null)
		// 	return;
		//
		// room.Push(room.HandleSkill, player, skillPacket);
	}
}
