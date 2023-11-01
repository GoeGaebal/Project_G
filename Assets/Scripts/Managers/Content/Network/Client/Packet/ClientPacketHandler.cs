﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

partial class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
		Managers.Object.RemoveMyPlayer();
		Managers.Network.Client.DisConnect();
		// if(Managers.Network.isHost) Managers.Network.Server.Close();
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		foreach (var obj in spawnPacket.Objects)
		{
			Managers.Object.Add(obj, myPlayer: false);
		}
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;
		
		// Debug.Log($"Receive: (id: {movePacket.ObjectId} {movePacket.PosInfo.PosX} , {movePacket.PosInfo.PosY})");
		
		GameObject go = Managers.Object.FindById(movePacket.ObjectId);
		if (go == null)
			return;
		
		// BaseController bc = go.GetComponent<BaseController>();
		// if (bc == null)
		// 	return;
		if (movePacket.ObjectId != Managers.Network.LocalPlayer.Id)
		{
			var p = go.GetComponent<Player>();
			p.Info.PosInfo = movePacket.PosInfo;
			p.SyncPos();
		}
		//go.PosInfo = movePacket.PosInfo;
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		// S_Skill skillPacket = packet as S_Skill;
		//
		// GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		// if (go == null)
		// 	return;
		//
		// CreatureController cc = go.GetComponent<CreatureController>();
		// if (cc != null)
		// {
		// 	cc.UseSkill(skillPacket.Info.SkillId);
		// }
	}

	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		// S_ChangeHp changePacket = packet as S_ChangeHp;
		//
		// GameObject go = Managers.Object.FindById(changePacket.ObjectId);
		// if (go == null)
		// 	return;
		//
		// CreatureController cc = go.GetComponent<CreatureController>();
		// if (cc != null)
		// {
		// 	cc.Hp = changePacket.Hp;
		// }
	}

	public static void S_DieHandler(PacketSession session, IMessage packet)
	{
		// S_Die diePacket = packet as S_Die;
		//
		// GameObject go = Managers.Object.FindById(diePacket.ObjectId);
		// if (go == null)
		// 	return;
		//
		// CreatureController cc = go.GetComponent<CreatureController>();
		// if (cc != null)
		// {
		// 	cc.Hp = 0;
		// 	cc.OnDead();
		// }
	}
	
	public static void S_ChatHandler(PacketSession session, IMessage packet)
	{
		S_Chat chat = packet as S_Chat;
		if (chat == null) return;
		if(Managers.Network.UIChat != null)
			Managers.Network.UIChat.UpdateChat(chat.Msg);
	}
	
	public static void S_LoadSceneHandler(PacketSession session, IMessage packet)
	{
		S_LoadScene loadScenePacket = packet as S_LoadScene;
		if (loadScenePacket == null) return;
		
		Managers.Scene.LoadScene(loadScenePacket.SceneType);
		Managers.Network.LocalPlayer.transform.position = new(loadScenePacket.PosX, loadScenePacket.PosY);
	}

	public static void S_WorldMapHandler(PacketSession session, IMessage packet)
	{
		if (Managers.Network.isHost) return;
		S_WorldMap worldMapPacket = packet as S_WorldMap;
		Managers.WorldMap.UpdateByPacket(worldMapPacket);
	}

	public static void S_WorldMapEventHandler(PacketSession session, IMessage packet)
	{
		if (Managers.Network.isHost) return;
		S_WorldMapEvent worldMapPacket = packet as S_WorldMapEvent;
		Managers.WorldMap.UI.UpdateByPacket(worldMapPacket);
	}

	public static void S_OnDamageHandler(PacketSession session, IMessage packet)
	{
		if (Managers.Network.isHost) return;
		S_OnDamage damagePacket = packet as S_OnDamage;
		DamageableEntity entity =  Managers.Object.FindById(damagePacket.ObjectId).GetComponent<DamageableEntity>();
		entity.UpdateHP(damagePacket.HP, damagePacket.IsDead);
		entity.OnDamage(damagePacket.Damage);
	}
	
	public static void S_AddItemHandler(PacketSession session, IMessage packet)
	{
		if (Managers.Network.isHost) return;
		S_AddItem itemPacket = packet as S_AddItem;
		if (itemPacket == null) return;
		
		UI_Inven.additem(Managers.Data.ItemDict[itemPacket.ItemId]);
		Managers.Object.Remove(itemPacket.ObjectId);
	}
}

