using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviourPun
{
    [SerializeField] private Image hpBar;
    
    private GameObject[] players;
    private GameObject playerGO;
    private Player player;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");//씬에 있는 플레이어들 중
        foreach (GameObject p in players)
        {
            PhotonView photonView = p.GetPhotonView();
            if (photonView != null && photonView.IsMine)//내 플레이어 오브젝트 찾기
            {
                playerGO = p;//플레이어 게임오브젝트
                player = playerGO.GetComponent<Player>();//플레이어 스크립트
            }
        }

        hpBar = GetComponent<Image>();
    }
    
    private void Update()
    {
        hpBar.fillAmount = player.HP / player.maxHP;
    }
}
