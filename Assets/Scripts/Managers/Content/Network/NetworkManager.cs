// TODO : 리팩토링 
// 1. 패킷을 큐 형태로 모아 보내기
// 2. Player들을 미리 찾아놓기
public class NetworkManager
{
    public readonly ServerManager Server = new ServerManager();
    public readonly ClientManager Client = new ClientManager();
    public UI_Chat UIChat { get; set; }
    public Player LocalPlayer { get; set; }

    public bool IsHost = false;

    public void Init()
    {
        Client.Init();
        Server.Init();
    }
    
    public void CreateRoom()
    {
        IsHost = true;
        Managers.Network.Server.Listen();
        Managers.Network.Client.Connect();
        InitWaitinRoom();
    }
    
    public void FindRoom()
    {
        IsHost = false;
        Managers.Network.Client.Connect();
        InitWaitinRoom();
    }

    private void InitWaitinRoom()
    {
        Managers.UI.Clear();
        Managers.UI.SetEventSystem();
        Managers.UI.ShowSceneUI<UI_Inven>();
        //Managers.UI.ShowSceneUI<UI_Map>();
        Managers.UI.ShowSceneUI<UI_Status>();
        Managers.UI.ShowSceneUI<UI_Chat>();
        Managers.UI.ShowSceneUI<UI_Leaf>();
        Managers.Map.LoadMap(5);
    }
}
