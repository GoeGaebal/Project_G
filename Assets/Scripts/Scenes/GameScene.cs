using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;
        Managers.Map.LoadMap(1);
    }

    private void Start()
    {
        Managers.Network.SpawnLocalPlayer();
        Managers.Object.SpawnGatherings(1,5);

        Managers.UI.SetEventSystem();
        Managers.UI.ShowSceneUI<UI_Inven>();
        Managers.UI.ShowSceneUI<UI_Map>();
        Managers.UI.ShowSceneUI<UI_Status>();
        Managers.UI.ShowSceneUI<UI_Chat>();
        
    }

    public override void Clear()
    {
        
    }
}
