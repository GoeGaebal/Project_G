using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class FinalBossScene : BaseScene
{
    public CinemachineTargetGroup playerCam;

    public Image fade;
    protected override void Init()
    {
        base.Init();
        SceneType = SceneType.Ship;
        Managers.Map.LoadMap(4);
        Managers.Input.PlayerActionMap.Enable();

        playerCam.AddMember(Managers.Network.LocalPlayer.transform, 1f, 7f);
        StartCoroutine(CamToBoss());
    }

    private void Start()
    {
        //if (!PhotonNetwork.IsConnected)
        //{
        //    Managers.UI.SetEventSystem();
        //    GameObject player = Managers.Resource.Instantiate("Player", Vector3.zero, Quaternion.identity);
        //    // 테스트용 강제 설정
        //    PhotonView view = player.GetComponent<PhotonView>();
        //    PhotonView[] weaponView = player.GetPhotonViewsInChildren();
        //    PhotonNetwork.AllocateViewID(view);
        //    PhotonNetwork.AllocateViewID(weaponView[1]);
        // }
        
        Managers.UI.SetEventSystem();
        Managers.UI.ShowSceneUI<UI_Status>();
        Managers.UI.ShowSceneUI<UI_Chat>();
        Managers.UI.ShowSceneUI<UI_Leaf>();
        Managers.UI.ShowSceneUI<UI_Inven>();
        Managers.UI.ShowSceneUI<UI_Crosshair>();

        var scene = Managers.UI.ShowSceneUI<UI_PopupText>();
        scene.Init();
    }

    IEnumerator CamToBoss()
    {
        yield return new WaitForSeconds(1f);
        playerCam.m_Targets[0].weight = 1f;
        playerCam.m_Targets[1].weight = 0f;

        yield return new WaitForSeconds(1.8f);
        playerCam.m_Targets[1].weight = 1f;
        playerCam.m_Targets[0].weight = 0f;

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        while(fade.color.a < 1f)
        {
            Color temp = fade.color;
            temp.a += Time.deltaTime * 0.75f;
            fade.color = temp;
            yield return null;
        }

        Managers.Scene.LoadScene(SceneType.Credit);
        yield break;
    }

    public override void Clear()
    {
        Managers.UI.Clear();
        Managers.WorldMap.FinishFinalBoss();
    }
}
