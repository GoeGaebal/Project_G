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
        Map_001,
        Map_002,
        Map_003,
        Worldmap_Button_Stop
    }
    //enum Lines { Worldmap_Line }
    
    private GameObject _ship;//배
    private GameObject _target;//이동 목표
    private GameObject _lrGO;
    private GameObject _worldmap;
    private UILineRenderer _lr;

    private bool _moveFlag = true;//이동 중
    private bool _arriveFlag = false;//도착

    private string _mapName; 

    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if (_moveFlag)
        {
            if (Vector2.Distance(_ship.transform.position, _target.transform.position) >= 1f)
            {
                MoveToTarget();
            }
            else
            {
                _arriveFlag = true;
            }
        }
        if (_arriveFlag && _mapName != "Worldmap_Ship")
        {
            //TODO: 필드로 이동 가능하게 하는 코드
            //_mapName 이용해서 해당하는 맵 프리팹을 필드에 생성시키기.
        }
    }

    public override void Init()
    {
        //TODO: Worldmap UI가 sorting order가 밀려서 밑으로 깔리는 문제 발생.
        //base.Init();
        Managers.UI.SetCanvas(gameObject, true);

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        _worldmap=Get<GameObject>((int)GameObjects.Worldmap_Background);
        _worldmap.SetActive(false);
        _ship = Get<GameObject>((int)GameObjects.Worldmap_Ship);
        _target = _ship;
        _lrGO = Get<GameObject>((int)GameObjects.Worldmap_Line);
        _lr = _lrGO.GetComponent<UILineRenderer>();

        GetButton((int)Buttons.Worldmap_Button).gameObject.BindEvent(OpenWorldmapUI);
        GetButton((int)Buttons.Worldmap_Button_Close).gameObject.BindEvent(CloseWorldmapUI);
        GetButton((int)Buttons.Map_001).gameObject.BindEvent(OnWorldmapButtonClick);
        GetButton((int)Buttons.Map_002).gameObject.BindEvent(OnWorldmapButtonClick);
        GetButton((int)Buttons.Map_003).gameObject.BindEvent(OnWorldmapButtonClick);
        GetButton((int)Buttons.Worldmap_Button_Stop).gameObject.BindEvent(PauseMove);
    }

    public void SetTarget(GameObject t)//목표 지정
    {
        _target = t;
        _mapName = t.name;
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
        _lr.Points[0] = _ship.transform.InverseTransformPoint(_ship.transform.position); ;
        _lr.Points[1] = _ship.transform.InverseTransformPoint(_target.transform.position);
        _lr.SetAllDirty();
    }

    public void OnWorldmapButtonClick(PointerEventData evt)
    {
        SetTarget(evt.pointerPress);
        _arriveFlag = false;
        _moveFlag = true;
        _lr.enabled = true;
    }

    public void PauseMove(PointerEventData evt)
    {
        SetTarget(_ship);
        _moveFlag = false;
        _lr.enabled = false;
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
