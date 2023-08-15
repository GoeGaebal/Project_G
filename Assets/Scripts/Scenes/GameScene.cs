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
    }

    public override void Clear()
    {
        
    }
}
