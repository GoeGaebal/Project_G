﻿using System;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

partial class PacketHandler
{
	public static void C_PlayerMoveHandler(PacketSession session, IMessage packet)
	{
		C_PlayerMove movePacket = packet as C_PlayerMove;
		ClientSession clientSession = session as ClientSession;
		if (movePacket == null || clientSession == null) return;
		
		Player player = clientSession.MyPlayer;
		if (player == null)  return;
		
		Managers.Network.Server.Room.HandleMove(player,movePacket);
	}

	public static void C_ChatHandler(PacketSession session, IMessage packet)
	{
		C_Chat chat = packet as C_Chat;
		ClientSession clientSession = session as ClientSession;
		if (chat == null || clientSession == null) return;
		
		S_Chat resChat = new S_Chat();
		resChat.Msg = $"{clientSession.MyPlayer.Info.Name}: {chat.Msg}";
		Managers.Network.Server.Room.Broadcast(resChat);
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

	public static void C_ArtifactEventHandler(PacketSession session, IMessage packet)
	{
		ClientSession clientSession = session as ClientSession;

		if (packet is not C_ArtifactEvent evt) return;
		S_ArtifactEvent resEvt = new S_ArtifactEvent()
		{
			CurrentId = evt.CurrentId,
			ArtifactId = evt.ArtifactId
		};
		Managers.Network.Server.Room.Broadcast(resEvt);
	}

	public static void C_ChangeNameHandler(PacketSession session, IMessage packet)
	{
		ClientSession clientSession = session as ClientSession;
		if (packet is not C_ChangeName evt) return;
		if (clientSession == null) return;
		
		Managers.Network.Server.Room.ChangeName(clientSession.MyPlayerId, evt.Name);
		Managers.Network.Server.Room.Broadcast(new S_ChangeName()
		{
			Name = evt.Name,
			ObjectId = clientSession.MyPlayerId
		});
	}

	public static void C_SpawnLootingHandler(PacketSession session, IMessage packet)
	{
		ClientSession clientSession = session as ClientSession;
		if (packet is not C_SpawnLooting evt) return;
		if (clientSession == null) return;
		
		Managers.Network.Server.Room.SpawnLootingItems(evt.ObjectId, evt.Count, evt.PlayerId, 2.0f,0.0f);
	}
}
