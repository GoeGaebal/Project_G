using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ShipScene : BaseScene
{
    public static bool isStarted = false;
    protected override void Init()
    {
        if(!isStarted) isStarted = true;
        base.Init();
        SceneType = SceneType.Ship;
        Managers.Map.LoadMap(4);

        if(Managers.Network.IsHost)
            Managers.Network.Server.Room.ReviveAll(5.0f);


        GameObject ui_LeafObj = GameObject.Find("UI_Leaf");
        if(ui_LeafObj != null) ui_LeafObj.GetComponent<UI_Leaf>().HealPlayers();
    }

    private void Start()
    {
        Managers.UI.SetEventSystem();
        UI_Scene ui = null;
        Managers.UI.ShowSceneUI<UI_Inven>();
        Managers.UI.ShowSceneUI<UI_Leaf>();
        Managers.UI.ShowSceneUI<UI_Chest>();
        Managers.UI.ShowSceneUI<UI_Chat>();
        Managers.UI.ShowSceneUI<UI_Artifact>();
        Managers.UI.ShowSceneUI<UI_Craft>();
        Managers.UI.ShowSceneUI<UI_Status>();
        Managers.UI.ShowSceneUI<UI_Worldmap>();
        Managers.UI.ShowSceneUI<UI_SystemMessage>();
        Managers.UI.ShowSceneUI<UI_Crosshair>();
        Managers.UI.ShowSceneUI<UI_PopupText>();
        // Managers.Network.ResetPlayer();
    }

    public override void Clear()
    {
        Managers.Object.ClearObjects(GameObjectType.Monster, GameObjectType.Gathering);
        Managers.UI.Clear();
    }
}
