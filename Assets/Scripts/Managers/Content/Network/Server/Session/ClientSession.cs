﻿using System;
using System.Collections;
using System.Collections.Generic;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Unity.VisualScripting;
using UnityEngine;

namespace Server
{
	public class ClientSession : PacketSession
	{
		public Player MyPlayer => Managers.Object.PlayerDict[_myPlayerId];
		private int _myPlayerId;

		public int SessionId { get; set; }

		public void OnConnectedCoroutine()
		{
			// MyPlayer = Managers.Resource.Instantiate("Objects/Character/Player").GetComponent<Player>();
			_myPlayerId = Managers.Object.GenerateId(GameObjectType.Player);
			ObjectInfo myPlayerInfo = new ObjectInfo();
			myPlayerInfo.ObjectId = _myPlayerId;
			myPlayerInfo.Name =  $"Player_{_myPlayerId}";
			Managers.Network.Server.Room.EnterGame(this, myPlayerInfo);
		}

		public void Send(IMessage packet)
		{
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
			Send(new ArraySegment<byte>(sendBuffer));
		}

		public override void OnConnected(EndPoint endPoint)
		{
			Debug.Log($"OnConnected : {endPoint}");
			Managers.Network.Server.JobQueue.Enqueue(OnConnectedCoroutine);
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			Managers.Network.Server.SPM.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			Managers.Network.Server.Room.LeaveGame(MyPlayer.Info.ObjectId);
			Managers.Network.Server.SessionManager.Remove(this);
			Debug.Log($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
