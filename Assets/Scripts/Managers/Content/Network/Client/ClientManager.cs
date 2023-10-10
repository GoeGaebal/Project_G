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
	private IPAddress _ipAddr;
	private IPEndPoint _endPoint;

	public void Init()
	{
		// DNS (Domain Name System)
		_host = Dns.GetHostName();
		_ipHost = Dns.GetHostEntry(_host);
		_ipAddr = _ipHost.AddressList[0];
		
		Managers.Network.Client.CPM.CustomHandler = (s, m, i) =>
		{
			Managers.Network.Client.PQ.Push(s, i, m);
		};
	}

	public void Connect(int port = 7777)
	{
		_endPoint = new IPEndPoint(_ipAddr, port);
		Connector connector = new Connector();

		connector.Connect(_endPoint,
			() => { return _session = new ServerSession(); },
			1);
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
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}	
	}

}
