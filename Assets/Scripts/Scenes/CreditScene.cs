using System.Collections;
using Google.Protobuf.Protocol;

public class CreditScene : BaseScene
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EndCoroutine());
    }

    protected override void Init()
    {
        base.Init();
        SceneType = SceneType.Credit;
    }

    IEnumerator EndCoroutine()
    {
        // Photon.Pun.PhotonNetwork.LeaveRoom();
        //yield return new WaitForSeconds(1f);

        //Managers.Scene.LoadScene(SceneType.Credit);
        yield break;
    }

    public override void Clear()
    {
        Managers.UI.Clear();
    }
}
