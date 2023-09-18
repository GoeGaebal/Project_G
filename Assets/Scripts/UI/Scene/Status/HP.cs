using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HP : MonoBehaviourPun
{
    private Image hpBar;
    private TextMeshPro hpText;

    private GameObject[] players;
    private GameObject playerGO;
    private Player player;

    private void Start()
    {
        player = FindPlayer();//players 배열을 검색해오지 못하는 버그
        Debug.Log(player);

        hpBar = gameObject.transform.GetChild(0).GetComponent<Image>();
        hpText = gameObject.transform.GetChild(1).GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateHPBar();
        UpdateHPText();
    }

    private Player FindPlayer()
    {
        players = GameObject.FindGameObjectsWithTag("Player");//씬에 있는 플레이어들 중
        Debug.Log(players.Length);
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

    private void UpdateHPBar()
    {
        if (player != null)
        {
            float temp = player.HP / player.maxHP;
            hpBar.fillAmount = temp;
        }
        
    }

    private void UpdateHPText()
    {
        if(player != null)
        {
            hpText.SetText(player.HP + " / " + player.maxHP);
        }
        
    }
}
