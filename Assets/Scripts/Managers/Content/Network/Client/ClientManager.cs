using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;
using Unity.VisualScripting;

public class ClientManager
{
	public ClientPacketManager CPM = new();
	public PacketQueue PQ = new();

	private ServerSession _session;

	public void Send(IMessage packet)
	{
		_session.Send(packet);
	}

	private string _host;
	private IPHostEntry _ipHost;
	private IPAddress _ipAddress;
	private IPEndPoint _endPoint;

	public void Init()
	{
		Managers.Network.Client.CPM.CustomHandler = (s, m, i) =>
		{
			Managers.Network.Client.PQ.Push(s, i, m);
		};
	}

	public void Connect(Action onConnectedSucceed = null, Action onConnectedFailed = null, int port = 7777)
	{
		_host = Dns.GetHostName();
		_ipHost = Dns.GetHostEntry(_host);
		_ipAddress = _ipHost.AddressList[0];
		_endPoint = new IPEndPoint(_ipAddress, port);
		Connector connector = new Connector();

		connector.Connect(_endPoint,
			() => { return _session = new ServerSession(); },
			onConnectedSucceed, onConnectedFailed, 1);
	}
	
	public void Connect(string hostNameOrAddress, Action onConnectedSucceed = null, Action onConnectedFailed = null ,int port = 7777)
	{
		_ipHost = Dns.GetHostEntry(hostNameOrAddress);
		_host = _ipHost.HostName;
		_ipAddress = _ipHost.AddressList[0];
		_endPoint = new IPEndPoint(_ipAddress, port);
		Connector connector = new Connector();
		connector.Connect(_endPoint,
			() => { return _session = new ServerSession(); },
			onConnectedSucceed, onConnectedFailed, 1);
	}

	public void DisConnect()
	{
		_session.Disconnect();
	}

	public void Update()
	{
		List<PacketMessage> list = Managers.Network.Client.PQ.PopAll();
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = CPM.GetPacketHandler(packet.Id);
			handler?.Invoke(_session, packet.Message);
		}	
	}

}
