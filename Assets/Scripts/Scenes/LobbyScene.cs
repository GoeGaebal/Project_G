using Google.Protobuf.Protocol;

public class LobbyScene : BaseScene
{
    private UI_Start _startScreen;
    private UI_Lobby _lobbyScreen;
    private UI_CreateRoomSetting _createRoomSettingPopup;
    private UI_Room _roomScreen;

    protected override void Init()
    {
        base.Init();
        SceneType = SceneType.Lobby;
        _startScreen = Managers.UI.ShowSceneUI<UI_Start>();
    }
    public override void Clear()
    {
        Managers.UI.Clear();
    }
}
