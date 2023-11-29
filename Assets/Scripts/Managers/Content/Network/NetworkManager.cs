// TODO : 리팩토링 
// 1. 패킷을 큐 형태로 모아 보내기
// 2. Player들을 미리 찾아놓기

using System;

public class NetworkManager
{
    public readonly ServerManager Server = new ServerManager();
    public readonly ClientManager Client = new ClientManager();
    public UI_Chat UIChat { get; set; }
    public Player LocalPlayer { get; set; }
    public string UserName;
    public bool IsHost;

    public void Init()
    {
        Client.Init();
        Server.Init();
    }
    
    public void CreateRoom(Action onConnectedSucceed, Action onConnectedFailed, int port = 7777)
    {
        IsHost = true;
        Managers.Network.Server.Listen(port);
        Managers.Network.Client.Connect(onConnectedSucceed, onConnectedFailed, port);
    }
}
