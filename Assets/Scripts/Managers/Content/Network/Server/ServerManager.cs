using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Google.Protobuf;
using Server;
using ServerCore;
using UnityEngine;

public class ServerManager
{
    public Queue<IEnumerator> JobQueue = new Queue<IEnumerator>();
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

    public void Init()
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
        
        SPM.CustomHandler += (s,m,i) => Managers.Network.Server.PQ.Push(s, i, m);

        _listener.Init(endPoint, () => { return SessionManager.Generate(); });
        Debug.Log("Listening...");
    }

    public void Update()
    {
        SessionManager.Update();
    }

    public void End()
    {
        _listener.Close();
    }
}
