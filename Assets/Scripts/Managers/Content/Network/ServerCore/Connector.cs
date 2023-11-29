using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ServerCore
{
	public class Connector
	{
		private Func<Session> _sessionFactory;
		private Action _onConnectedFailed;
		private Action _onConnectedSucceed;

		public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, Action onConnectedSucceed = null, Action onConnectedFailed = null , int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				// 휴대폰 설정
				Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				_sessionFactory = sessionFactory;

				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += OnConnectCompleted;
				args.RemoteEndPoint = endPoint;
				args.UserToken = socket;
				if(onConnectedFailed != null) _onConnectedFailed += onConnectedFailed;
				if(onConnectedSucceed != null) _onConnectedSucceed += onConnectedSucceed;
				RegisterConnect(args);
			}
		}

		private void RegisterConnect(SocketAsyncEventArgs args)
		{
			if (args.UserToken is not Socket socket) return;

			bool pending = socket.ConnectAsync(args);
			if (pending == false)
				OnConnectCompleted(null, args);
		}

		private void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.ConnectSocket);
				session.OnConnected(args.RemoteEndPoint);
				_onConnectedSucceed?.Invoke();
			}
			else
			{
				Debug.Log($"OnConnectCompleted Fail: {args.SocketError}");
				_onConnectedFailed?.Invoke();
			}
		}
	}
}
