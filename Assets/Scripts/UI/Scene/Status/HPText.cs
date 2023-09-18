using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPText : MonoBehaviourPun
{
    private TMP_Text hpText;

    private GameObject[] players;
    private GameObject playerGO;
    private Player player;

    private void Start()
    {
        player = FindPlayer();

        hpText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        UpdateHPText();
    }

    private Player FindPlayer()
    {
        players = GameObject.FindGameObjectsWithTag("Player");//씬에 있는 플레이어들 중
        foreach (GameObject p in players)
        {
            PhotonView photonView = p.GetPhotonView();
            if (photonView != null && photonView.IsMine)//내 플레이어 오브젝트 찾기
            {
                playerGO = p;//플레이어 게임오브젝트
                return playerGO.GetComponent<Player>();//플레이어 스크립트
            }
        }
        return null;
    }

    private void UpdateHPText()
    {
        if (player != null)
        {
            hpText.text = player.HP + " / " + player.maxHP;
        }

    }
}
