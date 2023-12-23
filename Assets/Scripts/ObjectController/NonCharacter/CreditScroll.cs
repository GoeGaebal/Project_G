using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CreditScroll : MonoBehaviour
{
    private ScrollRect _credit;
    private Scrollbar _bar;

    private float _scrollSpeed = 0.2f;

    void Start()
    {
        _credit = GetComponent<ScrollRect>();
        _bar = _credit.verticalScrollbar;

        _bar.value = 1f;

        StartCoroutine(ScrollCoroutine());
        
        if(!Managers.Network.IsHost) Managers.Network.Client.DisConnect();
        Managers.Network.IsEnd = true;
    }

    void Update()
    {

    }

    IEnumerator ScrollCoroutine()
    {
        if (_bar.value > 0.001f)
        {
            // 대기 시간을 고려하여 스크롤 값을 조절합니다.
            _bar.value -= _scrollSpeed * Time.deltaTime;

            // 한 프레임 대기
            yield return null;
            StartCoroutine(ScrollCoroutine());
        }
        else
        {
            StartCoroutine(LoadLobby());
            yield break;
        }
    }

    IEnumerator LoadLobby()
    {
        if (Managers.Input.UIActions.Click.IsPressed() || Managers.Input.UIActions.RightClick.IsPressed())
        {
            yield return null;
            StartCoroutine(LoadLobby());
        }
        // TODO: AnyKey가 없어서 일단 keyboard에서 직접 받도록 하였음
        else if (Keyboard.current.anyKey.IsPressed())
        {
            if(Managers.Network.IsHost) Managers.Network.Server.ShutDown();
            else Managers.Scene.LoadScene(SceneType.Intro);
            yield break;
        }
        else
        {
            yield return null;
            StartCoroutine(LoadLobby());
        }
    }
}
