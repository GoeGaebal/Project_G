using System;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using UnityEngine;

namespace Server
{
	public class ClientSession : PacketSession
	{
		public Player MyPlayer {
			get
			{
				Managers.Object.PlayerDict.TryGetValue(MyPlayerId, out var ret);
				return ret;
			}
		}
		
		public int MyPlayerId;

		public int SessionId { get; set; }

		public void OnConnectedCoroutine()
		{
			// MyPlayer = Managers.Resource.Instantiate("Objects/Character/Player").GetComponent<Player>();
			
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
			MyPlayerId = GameRoom.GenerateId(GameObjectType.Player);
			var myPlayerInfo = new ObjectInfo
			{
				ObjectId = MyPlayerId,
			};
			Managers.Network.Server.Room.EnterGame(this, myPlayerInfo);
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
