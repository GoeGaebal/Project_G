using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Ship;
        Managers.Map.LoadMap(4);
    }

    private void Start()
    {
        Managers.Network.SpawnLocalPlayer(Vector3.zero);
        // Managers.Object.SpawnGatherings(1,5);

        Managers.UI.SetEventSystem();
        Managers.UI.ShowSceneUI<UI_Inven>();
        Managers.UI.ShowSceneUI<UI_Map>();
        Managers.UI.ShowSceneUI<UI_Status>();
        Managers.UI.ShowSceneUI<UI_Chat>();
        
    }

    public override void Clear()
    {
        // throw new System.NotImplementedException();
    }
}
