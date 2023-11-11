using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Server;
using UnityEngine;

namespace ServerCore
{
	public class Listener
	{
		Socket _listenSocket;
		Func<Session> _sessionFactory;

		public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
		{
			_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_sessionFactory += sessionFactory;

			// 문지기 교육
			_listenSocket.Bind(endPoint);

			// 영업 시작
			// backlog : 최대 대기수
			_listenSocket.Listen(backlog);

			for (int i = 0; i < register; i++)
			{
				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
				RegisterAccept(args);
			}
		}

		void RegisterAccept(SocketAsyncEventArgs args)
		{
			args.AcceptSocket = null;

			try
			{
				bool pending = _listenSocket.AcceptAsync(args);
				if (pending == false)
					OnAcceptCompleted(null, args);
			}
			catch (ObjectDisposedException ex)
			{
				Debug.Log($"ObjectDisposeException : {ex}");
			}
			
		}

		void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.AcceptSocket);
				session.OnConnected(args.AcceptSocket.RemoteEndPoint);
			}
			else
				Debug.Log(args.SocketError.ToString());

			RegisterAccept(args);
		}

		public void ShutDown()
		{
			_listenSocket.Close();
			_listenSocket.Dispose();
			// _listenSocket.Shutdown(SocketShutdown.Both);
			_sessionFactory = null;
		}
	}
}
