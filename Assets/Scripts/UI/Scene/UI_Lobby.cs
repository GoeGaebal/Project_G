using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Lobby : UI_Popup
{

    public Button CreateBtn { get; private set; }
    public Button RedoBtn { get; private set; }
    public GameObject LoadingPane { get; private set; }
    public List<UI_RoomItem> RoomList = new List<UI_RoomItem>();
    
    private GameObject _content;

    enum Buttons
    {
        CreateBtn,
        FindBtn,
        RedoBtn,
    }

    enum GameObjects
    {
        Content,
        LoadingPane,
    }

    
    private void Start()
    {
        if(CreateBtn == null)
            Init();
    }

    public override void Init()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        CreateBtn = GetButton((int)Buttons.CreateBtn);
        RedoBtn = GetButton((int)Buttons.RedoBtn);

        _content = GetObject((int)GameObjects.Content);
        LoadingPane = GetObject((int)GameObjects.LoadingPane);
        // GetButton((int)Buttons.CreateBtn).gameObject.BindEvent((evt) => { Managers.UI.ShowPopupUI<UI_CreateRoom>();});
        LoadingPane.SetActive(false);
    }

    public UI_RoomItem AddRoom()
    {
        var room = Managers.UI.MakeSubItem<UI_RoomItem>(parent: _content.transform);
        room.transform.localScale = Vector3.one;
        RoomList.Add(room);
        return room;
    }

    public void RemoveRoom()
    {
        if (RoomList.Count > 0)
        {
            var room = RoomList[^1];
            RoomList.RemoveAt(RoomList.Count-1);
            Managers.Resource.Destroy(room.gameObject);
        }
    }
}
