using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using ServerCore;
using UnityEngine;

public class PacketMessage
{
	public PacketSession PacketSession;
	public ushort Id { get; set; }
	public IMessage Message { get; set; }
}

public class PacketQueue
{
	object _lock = new object();
	Queue<PacketMessage> _packetQueue = new();

	public void Push(PacketSession session, ushort id, IMessage packet)
	{
		lock (_lock)
		{
			_packetQueue.Enqueue(new PacketMessage() { PacketSession = session, Id = id, Message = packet });
		}
	}

	public PacketMessage Pop()
	{
		lock (_lock)
		{
			if (_packetQueue.Count == 0)
				return null;

			return _packetQueue.Dequeue();
		}
	}

	public List<PacketMessage> PopAll()
	{
		List<PacketMessage> list = new List<PacketMessage>();

		lock (_lock)
		{
			while (_packetQueue.Count > 0)
				list.Add(_packetQueue.Dequeue());
		}

		return list;
	}
}