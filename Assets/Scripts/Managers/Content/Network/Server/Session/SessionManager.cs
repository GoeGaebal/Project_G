using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using ServerCore;
using UnityEngine;

namespace Server
{
	public class SessionManager
	{
		int _sessionId = 0;
		Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
		object _lock = new object();

		public ClientSession Generate()
		{
			lock (_lock)
			{
				int sessionId = ++_sessionId;

				ClientSession session = new ClientSession();
				session.SessionId = sessionId;
				_sessions.Add(sessionId, session);

				Debug.Log($"Connected : {sessionId}");

				return session;
			}
		}

		public ClientSession Find(int id)
		{
			lock (_lock)
			{
				ClientSession session = null;
				_sessions.TryGetValue(id, out session);
				return session;
			}
		}

		public void Remove(ClientSession session)
		{
			lock (_lock)
			{
				_sessions.Remove(session.SessionId);
			}
		}

		public void Close()
		{
			lock (_lock)
			{
				_sessions.Clear();
			}
		}

		public void Update()
		{
			List<PacketMessage> list = Managers.Network.Server.PQ.PopAll();
			foreach (PacketMessage packet in list)
			{
				Action<PacketSession, IMessage> handler = Managers.Network.Server.SPM.GetPacketHandler(packet.Id);
				if (handler != null)
					handler.Invoke(packet.PacketSession, packet.Message);
			}	
		}
	}
}
