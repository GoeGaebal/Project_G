using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Google.Protobuf;
using Server;
using ServerCore;
using Unity.VisualScripting;
using UnityEngine;

public class ServerManager
{
    public Queue<Action> JobQueue = new Queue<Action>();
    public GameRoom Room = new GameRoom();
    public SessionManager SessionManager = new SessionManager();
    public ServerPacketManager SPM = new ServerPacketManager();
    public PacketQueue PQ = new PacketQueue();
    static Listener _listener = new Listener();
    static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();
    
    // static void TickRoom(GameRoom room, int tick = 100)
    // {
    //     var timer = new System.Timers.Timer();
    //     timer.Interval = tick;
    //     timer.Elapsed += ((s, e) => { room.Update(); });
    //     timer.AutoReset = true;
    //     timer.Enabled = true;
    //
    //     _timers.Add(timer);
    // }

    private string _host;
    private IPHostEntry _ipHost;
    private IPAddress _ipAddr;
    private IPEndPoint _endPoint;
    private bool _isListening = false;

    public void Init()
    {
        // DNS (Domain Name System)
        _host = Dns.GetHostName();
        _ipHost = Dns.GetHostEntry(_host);
        _ipAddr = _ipHost.AddressList[0];
        
        SPM.CustomHandler += (s,m,i) => Managers.Network.Server.PQ.Push(s, i, m);
    }

    public bool Listen(int port = 7777)
    {
        if (_isListening) return false;
        _endPoint = new IPEndPoint(_ipAddr, port);
        _listener = new Listener();
        if (!_listener.Init(_endPoint, () => SessionManager.Generate())) return false;
        Debug.Log("Listening...");
        _isListening = true;
        return _isListening;
    }

    public void Update()
    {
        SessionManager.Update();
    }

    public void ShutDown()
    {
        SessionManager.Close();
        _listener.ShutDown();
        _isListening = false;
    }
}
