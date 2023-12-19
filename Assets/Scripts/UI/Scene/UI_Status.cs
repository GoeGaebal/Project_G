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
    enum GameObjects
    {
        Timer,
        OtherStatus
    }
    enum Images { HPBar }
    enum Texts { HPText }
    enum Buttons 
    { 
        OptionButton
    }

    private Image hpBar;
    private TextMeshProUGUI hpText;

    private GameObject[] players;
    private GameObject playerGO;

    private GameObject rotatingTimer;
    private float ratio = 0;

    private readonly List<UI_OtherHP> _uiOtherHps = new List<UI_OtherHP>();

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
        Bind<Button>(typeof(Buttons));

        hpBar = GetImage((int)Images.HPBar);
        hpText = GetTextMeshPro((int)Texts.HPText);

        rotatingTimer = GetObject((int)GameObjects.Timer);

        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OpenOption);
    }
    
    private void Update()
    {
        UpdateHPBar();
        UpdateOtherHp();
        UpdateHPText();
    }

    void FixedUpdate()
    {
        AddTime();
    }

    private void UpdateHPBar()
    {
        if (Managers.Network.LocalPlayer != null)
        {
            float temp = Managers.Network.LocalPlayer.HP / Managers.Network.LocalPlayer.maxHP;
            hpBar.fillAmount = temp;
        }
    }

    private void UpdateOtherHp()
    {
        int i;
        if (_uiOtherHps.Count != Managers.Object.OtherPlayerDict.Count)
        {
            foreach(var ui in _uiOtherHps) Managers.Resource.Destroy(ui.gameObject);
            _uiOtherHps.Clear();
            for (i = 0; i < Managers.Object.OtherPlayerDict.Count; i++)
            {
                _uiOtherHps.Add(Managers.UI.MakeSubItem<UI_OtherHP>(parent: GetObject((int)GameObjects.OtherStatus).transform));
                _uiOtherHps[i].Init();
            }
        }

        i = 0;
        foreach (var player in Managers.Object.OtherPlayerDict.Values) _uiOtherHps[i++].SetStatus(player);
    }

    private void UpdateHPText()
    {
        if(Managers.Network.LocalPlayer != null)
        {
            hpText.SetText(Managers.Network.LocalPlayer.HP + " / " + Managers.Network.LocalPlayer.maxHP);
        }
    }
    
    private void AddTime()
    {
        rotatingTimer.transform.rotation *= Quaternion.Euler(0f, 0f, 180f * ratio);
    }

    private void OpenOption(PointerEventData evt)
    {
        Managers.UI.ShowPopupUI<UI_Option>();
        Managers.Option.isOptionPanelOn = true;
        Managers.Input.PlayerActionMap.Disable();
    }
}
