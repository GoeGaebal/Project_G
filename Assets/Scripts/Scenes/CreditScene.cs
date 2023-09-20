using System.Collections;
using System.Collections.Generic;
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
        Photon.Pun.PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(3f);
        
        Managers.Scene.LoadScene(Define.Scene.Lobby);
        
        
        
    }

    public override void Clear()
    {
        
    }

}
