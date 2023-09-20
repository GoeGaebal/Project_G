using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_PopupText : UI_Scene
{
    private Dictionary<Transform, UI_PopupNameItem> _nameItems;
    private UI_PopupNameItem[] _nameItemList;
    public override void Init()
    {
        base.Init();

        _nameItems ??= new();
        _nameItemList ??= new[]
        {
            Managers.UI.MakeSubItem<UI_PopupNameItem>(parent: transform),
            Managers.UI.MakeSubItem<UI_PopupNameItem>(parent: transform),
            Managers.UI.MakeSubItem<UI_PopupNameItem>(parent: transform)
        };

        foreach (var name in _nameItemList)
        {
            name.gameObject.SetActive(false);
        }

        if (Managers.Network.PlayerList != null)
        {
            for (int i = 0; i < Managers.Network.PlayerList.Length; i++)
            {
                _nameItemList[i].Init();
                _nameItemList[i].target = Managers.Network.PlayerList[i].transform;
                _nameItems.Add(Managers.Network.PlayerList[i].transform,_nameItemList[i]);
            }
        }
        
        Managers.Network.AfterPlayerEnteredRoom -= AddNames;
        Managers.Network.AfterPlayerEnteredRoom += AddNames;
        Managers.Network.OnPlayerLeftRoomAction -= DeleteName;
        Managers.Network.OnPlayerLeftRoomAction += DeleteName;
    }
    
    

    public void AddName(int newPlayerActNr)
    {
        if (PhotonNetwork.IsConnected)
        {
            UI_PopupNameItem item = null;
            Transform targetTransform = Managers.Network.PlayerDict[newPlayerActNr].transform;
            if (!_nameItems.TryGetValue(targetTransform, out item))
            {
                foreach (var uiPopupNameItem in _nameItemList)
                {
                    if (uiPopupNameItem.target != null) continue;
                    
                    uiPopupNameItem.target = targetTransform;
                    _nameItems.Add(targetTransform,uiPopupNameItem);
                    item = uiPopupNameItem;
                    break;
                }
            }

            if (item == null) return;
            item.Name.SetText(PhotonNetwork.CurrentRoom.GetPlayer(newPlayerActNr).NickName);
            item.gameObject.SetActive(true);
        }
    }
    
    private void DeleteName(int leftPlayerActNr)
    {
        if (PhotonNetwork.IsConnected)
        {
            _nameItems[Managers.Network.PlayerDict[leftPlayerActNr].transform].gameObject.SetActive(false);
        }
    }
    
    public void AddNames(Player p)
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            AddName(player.Key);
        }
    }

    public void OnDestroy()
    {
        Managers.Network.AfterPlayerEnteredRoom -= AddNames;
        Managers.Network.OnPlayerLeftRoomAction -= DeleteName;
    }
}
