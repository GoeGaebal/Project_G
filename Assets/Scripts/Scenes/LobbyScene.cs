using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

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
