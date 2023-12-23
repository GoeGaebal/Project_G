using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
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
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            yield return null;
            StartCoroutine(LoadLobby());
        }
        else if (Input.anyKeyDown)
        {
            Managers.Scene.LoadScene(SceneType.Lobby);
            yield break; ;
        }
        else
        {
            yield return null;
            StartCoroutine(LoadLobby());
        }
    }
}
