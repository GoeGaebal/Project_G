﻿using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Photon.Pun;
using UnityEngine;

public class GameScene : BaseScene
{

    static GameScene _thisScene = null;
    private static int _playerLifeCnt;
    public static int PlayerLifeCnt{
        get{return _playerLifeCnt;}
        set{
            Debug.Log("PlayerLifeCnt changed"+ _playerLifeCnt);
            _playerLifeCnt = value;
            if(_thisScene != null &&_playerLifeCnt == 0)
            {
                _thisScene.StartCoroutine(_thisScene.FieldGameOverCoroutine());
            }
        }
    }
    protected override void Init()
    {
        _thisScene = this;
        base.Init();
        SceneType = SceneType.Game;
        
        Managers.Map.LoadMap(Managers.WorldMap.currentMapId);
        Managers.Sound.Play("Plane_BGM", Define.Sound.Bgm);
    }

    private void Start()
    {
        Managers.UI.SetEventSystem();
        Managers.UI.ShowSceneUI<UI_Inven>();
        Managers.UI.ShowSceneUI<UI_Map>();
        Managers.UI.ShowSceneUI<UI_Status>();
        Managers.UI.ShowSceneUI<UI_Chat>();
        Managers.UI.ShowSceneUI<UI_SystemMessage>();
        Managers.UI.ShowSceneUI<UI_Crosshair>();

        _playerLifeCnt = Managers.Object.PlayerDict.Count;
        foreach (var player in Managers.Object.PlayerDict.Values)
        {
            player.transform.position = Vector3.zero;
        }
    }

    public override void Clear()
    {
        Managers.Object.ClearObjects(GameObjectType.Monster, GameObjectType.Gathering);
    }

    IEnumerator FieldGameOverCoroutine()
    {
        //게임 종료 UI 띄우고 배로 복귀 2초 기다린 다음에 복귀
        //Managers.UI.ShowSceneUI<>();
        yield return new WaitForSeconds(3.0f);
        UI_Leaf.AvailableCount--;
        Managers.Scene.LoadScene(SceneType.Ship);
    }
}
