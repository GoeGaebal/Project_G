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
        Managers.UI.SetEventSystem();
    }

    public override void Clear()
    {
        // throw new System.NotImplementedException();
    }
}
