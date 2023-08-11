using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_NameText : UI_Scene
{
    
    public override void Init()
    {
        base.Init();
    }

    public void AddName(int actorNumber)
    {
        PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        Managers.UI.MakeSubItem<UI_Name>();
    }
}
