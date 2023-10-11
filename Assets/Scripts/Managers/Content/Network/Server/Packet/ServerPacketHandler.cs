using System;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

partial class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;
		if (movePacket == null || clientSession == null) return;
		
		Player player = clientSession.MyPlayer;
		if (player == null)  return;

		GameRoom room = player.Room;
		if (room == null) return;

		room.HandleMove(player,movePacket);
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
}
