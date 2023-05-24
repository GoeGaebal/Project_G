using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Status : UI_Scene
{
    enum GameObjects { Timer }
    enum Images { HPBar }
    enum Texts { HPText }

    private Image hpBar;
    private TextMeshProUGUI hpText;

    private GameObject[] players;
    private GameObject playerGO;
    private Player player;
    
    private GameObject rotatingTimer;
    private float ratio = 0;
    

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        hpBar = GetImage((int)Images.HPBar);
        hpText = GetTextMeshPro((int)Texts.HPText);

        rotatingTimer = GetObject((int)GameObjects.Timer);

        player = FindPlayer(); //players 배열을 검색해오지 못하는 버그
    }
    
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
    
    void FixedUpdate()
    {
        AddTime();
    }
    
    private void AddTime()
    {
        rotatingTimer.transform.rotation *= Quaternion.Euler(0f, 0f, 180f * ratio);
    }
}
