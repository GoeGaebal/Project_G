﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene 관련 매니저
/// </summary>
public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    /// <summary>
    /// Scene을 불러오는 함수, 기존 Unity 방식으로는 type에 따라서 불러오지 못하므로 이를 반영하였다.
    /// </summary>
    /// <remark>
    /// PUN2의 모든 클라이언트들이 같은 씬을 로드하도록 하려면 PhotonNetwork.LoadLevel을 사용하여야 한다.
    /// </remark>
    /// <param name="type">
    /// Define에 존재하는 Scene enum class에 존재하는 상수
    /// /param>
	public void LoadScene(Define.Scene type)
    {
        Managers.Clear();

        SceneManager.LoadScene(GetSceneName(type));
        // PhotonNetwork.LoadLevel(GetSceneName(type));
    }
    
    /// <summary>
    /// Scene을 불러오는 함수, 기존 Unity 방식으로는 type에 따라서 불러오지 못하므로 이를 반영하였다.
    /// </summary>
    /// <remark>
    /// PUN2의 모든 클라이언트들이 같은 씬을 로드하도록 하는 PhotonNetwork.LoadLevel를 사용한다.
    /// </remark>
    /// <param name="type">
    /// Define에 존재하는 Scene enum class에 존재하는 상수
    /// /param>
    public void LoadLevel(Define.Scene type)
    {
        Managers.Clear();
        
        PhotonNetwork.LoadLevel(GetSceneName(type));
    }

    private string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}