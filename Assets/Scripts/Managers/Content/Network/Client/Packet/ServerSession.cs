using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Net;
using UnityEngine;

public class ServerSession : PacketSession
{
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
	}

	public override void OnDisconnected(EndPoint endPoint)
	{
		Debug.Log($"OnDisconnected : {endPoint}");
		Managers.Scene.LoadScene(Define.Scene.Lobby);
		Managers.Object.Clear();
	}

	public override void OnRecvPacket(ArraySegment<byte> buffer)
	{
		Managers.Network.Client.CPM.OnRecvPacket(this, buffer);
	}

	public override void OnSend(int numOfBytes)
	{
		//Console.WriteLine($"Transferred bytes: {numOfBytes}");
	}
}