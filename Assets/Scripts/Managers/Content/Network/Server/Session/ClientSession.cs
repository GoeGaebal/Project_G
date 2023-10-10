using System;
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
		public Player MyPlayer { get; set; }
		public PacketQueue pq = new PacketQueue();
		
		public int SessionId { get; set; }

		IEnumerator OnConnectedCoroutine()
		{
			MyPlayer = Managers.Resource.Instantiate("Objects/Character/Player").GetComponent<Player>();
			MyPlayer.Id = Managers.Object.GenerateId(GameObjectType.Player);
			MyPlayer.Info.Name = $"Player_{MyPlayer.Info.ObjectId}";
			MyPlayer.gameObject.name = MyPlayer.Info.Name;
			// MyPlayer.Info.PosInfo.State = CreatureState.Idle;
			// MyPlayer.Info.PosInfo.MoveDir = MoveDir.Down;
			// MyPlayer.Info.PosInfo.PosX = 0;
			// MyPlayer.Info.PosInfo.PosY = 0;
			MyPlayer.Session = this;
			Managers.Network.Server.Room.EnterGame(MyPlayer);
			yield break;
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
			Managers.Network.Server.JobQueue.Enqueue(OnConnectedCoroutine());
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			Managers.Network.Server.SPM.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			MyPlayer.Room.LeaveGame(MyPlayer.Info.ObjectId);

			Managers.Network.Server.SessionManager.Remove(this);

			Debug.Log($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
