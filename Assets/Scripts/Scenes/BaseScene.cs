using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public abstract class BaseScene : MonoBehaviourPunCallbacks
{
    public SceneType SceneType { get; protected set; } = SceneType.Unknown;

	void Awake()
	{
		Init();
	}

	protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
        {
            Debug.Log("UI/EventSystem 이 존재하지 않습니다.");
        }
        // Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
        Managers.Sound.Clear();
    }

    public abstract void Clear();
}
