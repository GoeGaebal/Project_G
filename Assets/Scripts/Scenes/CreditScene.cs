using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using Photon;

public class CreditScene : BaseScene
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EndCoroutine());
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
        
    }

}
