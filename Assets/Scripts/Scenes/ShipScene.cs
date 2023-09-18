using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShipScene : BaseScene
{
    public static bool isStarted = false;
    protected override void Init()
    {
        if(!isStarted) isStarted = true;
        base.Init();
        SceneType = Define.Scene.Ship;
        Managers.Map.LoadMap(4);

        // GameObject player = Managers.Resource.Instantiate("Creature/Player");
        // player.name = "Player";
        // Managers.Object.Add(player);
        //
        // for (int i = 0; i < 5; i++)
        // {
        //     GameObject monster = Managers.Resource.Instantiate("Creature/Monster");
        //     monster.name = $"Monster_{i+1}";
        //     
        //     // 랜덤 위치 스폰 (일단 겹쳐도 OK)
        //     Vector3Int pos = new Vector3Int()
        //     {
        //         x = Random.Range(-15, 15),
        //         y = Random.Range(-10, 10)
        //     };
        //
        //     MonsterController mc = monster.GetComponent<MonsterController>();
        //     mc.CellPos = pos;
        //     
        //     Managers.Object.Add(monster);
        // }
        //Managers.UI.ShowSceneUI<UI_Inven>();
        //Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        //gameObject.GetOrAddComponent<CursorController>();

        //GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        //Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);

        ////Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        //GameObject go = new GameObject { name = "SpawningPool" };
        //SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //pool.SetKeepMonsterCount(2);
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Managers.UI.SetEventSystem();
            GameObject player = Managers.Resource.Instantiate("Player", Vector3.zero, Quaternion.identity);
            // 테스트용 강제 설정
            PhotonView view = player.GetComponent<PhotonView>();
            PhotonView[] weaponView = player.GetPhotonViewsInChildren();
            PhotonNetwork.AllocateViewID(view);
            PhotonNetwork.AllocateViewID(weaponView[1]);
        }
        
        Managers.UI.SetEventSystem();
        Managers.UI.ShowSceneUI<UI_Inven>();
        //Managers.UI.ShowSceneUI<UI_Map>();
        Managers.UI.ShowSceneUI<UI_Status>();
        Managers.UI.ShowSceneUI<UI_Chat>();
        Managers.UI.ShowSceneUI<UI_Worldmap>();
        Managers.UI.ShowSceneUI<UI_Chest>();
        Managers.UI.ShowSceneUI<UI_Artifact>();
        Managers.UI.ShowSceneUI<UI_Craft>();
        Managers.UI.ShowSceneUI<UI_Leaf>();
     
        var scene = Managers.UI.ShowSceneUI<UI_PopupText>();
        scene.Init();
        scene.AddNames(null);
    }

    public override void Clear()
    {
        // throw new System.NotImplementedException();
    }
}
