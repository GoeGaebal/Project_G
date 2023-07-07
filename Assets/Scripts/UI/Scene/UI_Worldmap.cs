using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;

public class UI_Worldmap : UI_Scene
{
    //enum Images { Worldmap_Background }
    enum GameObjects {
        Worldmap_Background,
        Worldmap_Ship,
        Worldmap_Line
    }
    enum Buttons
    {
        Worldmap_Button,
        Worldmap_Button_Close,
        Worldmap_Button_Map001,
        Worldmap_Button_Map002,
        Worldmap_Button_Map003,
        Worldmap_Button_Stop
    }
    //enum Lines { Worldmap_Line }
    
    private GameObject _ship;//배
    private GameObject _target;//이동 목표
    private GameObject _lrGO;
    private GameObject _worldmap;
    UILineRenderer lr;

    private bool moveFlag = true;

    void Start()
    {
        Init();
        /*
        _lrGO = transform.Find("Worldmap_Background/Worldmap_Line").gameObject;
        lr = _lrGO.GetComponent<UILineRenderer>();
        _ship = transform.Find("Worldmap_Background/Worldmap_Ship").gameObject;
        _target = _ship;
        */
    }

    private void FixedUpdate()
    {
        if (moveFlag)
        {
            if (Vector2.Distance(_ship.transform.position, _target.transform.position) >= 1f)
            {
                MoveToTarget();
            }
        }
    }

    public override void Init()
    {
        //TODO: Worldmap UI가 sorting order가 밀려서 밑으로 깔리는 문제 발생.
        //base.Init();
        Managers.UI.SetCanvas(gameObject, true);

        //Bind<UILineRenderer>(typeof(Lines));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        //Bind<Image>(typeof(Images));

        //lr = Get<UILineRenderer>((int)Lines.Worldmap_Line);
        _worldmap=Get<GameObject>((int)GameObjects.Worldmap_Background);
        _worldmap.SetActive(false);
        _ship = Get<GameObject>((int)GameObjects.Worldmap_Ship);
        _target = _ship;
        _lrGO = Get<GameObject>((int)GameObjects.Worldmap_Line);
        lr = _lrGO.GetComponent<UILineRenderer>();

        GetButton((int)Buttons.Worldmap_Button).gameObject.BindEvent(OpenWorldmapUI);
        GetButton((int)Buttons.Worldmap_Button_Close).gameObject.BindEvent(CloseWorldmapUI);
        GetButton((int)Buttons.Worldmap_Button_Map001).gameObject.BindEvent(OnWorldmapButtonClick);
        GetButton((int)Buttons.Worldmap_Button_Map002).gameObject.BindEvent(OnWorldmapButtonClick);
        GetButton((int)Buttons.Worldmap_Button_Map003).gameObject.BindEvent(OnWorldmapButtonClick);
        GetButton((int)Buttons.Worldmap_Button_Stop).gameObject.BindEvent(PauseMove);
    }

    public void SetTarget(GameObject t)//목표 지정
    {
        _target = t;
    }

    private void MoveToTarget()//목표 방향으로 이동
    {
        float distance = Vector2.Distance(_ship.transform.position, _target.transform.position);
        Vector2 direction = _target.transform.position - _ship.transform.position;
        _ship.transform.position += (Vector3)(direction / distance);
        SetLine();
    }

    private void SetLine()//배와 목표 사이 선 새로 긋기
    {
        _lrGO.transform.position = _ship.transform.position;
        lr.Points[0] = _ship.transform.InverseTransformPoint(_ship.transform.position); ;
        lr.Points[1] = _ship.transform.InverseTransformPoint(_target.transform.position);
        lr.SetAllDirty();
    }

    public void OnWorldmapButtonClick(PointerEventData evt)
    {
        moveFlag = true;
        lr.enabled = true;
        SetTarget(evt.pointerPress);
    }

    public void PauseMove(PointerEventData evt)
    {
        SetTarget(_ship);
        moveFlag = false;
        lr.enabled = false;
    }
    public void OpenWorldmapUI(PointerEventData evt)
    {
        _worldmap.SetActive(true);
    }

    public void CloseWorldmapUI(PointerEventData evt)
    {
        _worldmap.SetActive(false);
    }
}
