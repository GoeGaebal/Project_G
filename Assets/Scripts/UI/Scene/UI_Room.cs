using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Room : UI_Popup
{
    enum Texts
    {
        MyName,
    }

    enum Buttons
    {
        StartBtn,
        ExitBtn,
    }

    enum GameObjects
    {
        FriendList,
    }

    public TextMeshProUGUI MyName;
    public Button StartBtn, ExitBtn;
    public UI_Friend[] Friends;

    public override void Init()
    {
        base.Init();

        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        MyName = GetTextMeshPro((int)Texts.MyName);
        StartBtn = GetButton((int)Buttons.StartBtn);
        ExitBtn = GetButton((int)Buttons.ExitBtn);
        Friends = GetObject((int)GameObjects.FriendList).transform.GetComponentsInChildren<UI_Friend>();
        foreach (var friend in Friends)
            friend.Init();
    }
}
