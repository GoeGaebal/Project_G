using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UI_Crosshair : UI_Scene
{
    enum GameObjects
    {
        Cursor,
    }

    [SerializeField] private Sprite cursor;
    [SerializeField] private Sprite crossHair;
    private GameObject _cursor;

    public override void Init()
    {
        //base.Init();
        Managers.UI.SetCanvas(gameObject, true);

        Bind<GameObject>(typeof(GameObjects));
        _cursor = Get<GameObject>((int)GameObjects.Cursor);
        Cursor.visible = false;
        
        _cursor.GetComponent<Image>().sprite = crossHair;
    }

    // Update is called once per frame
    void Update()
    {
        _cursor.transform.position = Input.mousePosition;

    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}
