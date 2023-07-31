using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
using TMPro;

public class UI_Worldmap : UI_Scene
{
    //enum Images { Worldmap_Background }

    enum GameObjects {
        Worldmap_Background,
        Worldmap_Ship,
        Worldmap_Line,
        WeatherText,
        Worldmap_Minimap_Background,
        Worldmap_Minimap_Arrow,
        Worldmap_Minimap_Ship,
        Worldmap_Minimap_MovingText,
        Worldmap_Minimap_TimeText,
        Worldmap_Minimap_DistanceText
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
    private Sprite _rightShip;//배 오른쪽 이미지
    private Sprite _leftShip;//배 왼쪽 이미지
    private GameObject _target;//이동 목표
    private float _elapsedTime = 0f;
    private int _speed = 50;//초당 이동 거리
    private float _timePerMeter;//거리 1 이동하는데 걸리는 시간
    private GameObject _lrGO;
    private GameObject _worldmap;
    private UILineRenderer _lr;
    private GameObject _weatherText;
    private GameObject _arrow;
    private GameObject _minimapShip;
    private GameObject _movingText;
    private GameObject _timeText;
    private GameObject _distanceText;

    private bool _moveFlag = false;//이동 중
    private bool _arriveFlag = false;//도착

    private string _mapName;

    void Start()
    {
        Init();
    }

    private void Update()
    {
        if (_moveFlag)
        {
            if (Vector2.Distance(_ship.transform.position, _target.transform.position) >= 1f)
            {
                MoveToTarget();
                _movingText.GetComponent<TMP_Text>().SetText(Managers.Data.WorldmapDict[_target.name[6] - '0'].name + "로 이동 중");
                if (!CheckWeather())
                {
                    _weatherText.GetComponent<TMP_Text>().SetText("Nothing Detected!");
                }
            }
            else
            {
                _moveFlag = false;
                _arriveFlag = true;
                _timeText.SetActive(false);
                _distanceText.SetActive(false);
                _arrow.SetActive(false);
            }
        }
        if (_arriveFlag && _mapName != "Worldmap_Ship")
        {
            _movingText.GetComponent<TMP_Text>().SetText(Managers.Data.WorldmapDict[_target.name[6] - '0'].name + "에 정박 중");
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
        _rightShip = Managers.Resource.Load<Sprite>("Art/UI/WorldMap/Worldmap_Ship");
        _leftShip = Managers.Resource.Load<Sprite>("Art/UI/WorldMap/Worldmap_Ship_Left");
        _target = _ship;
        _timePerMeter = 1f / _speed;
        _lrGO = Get<GameObject>((int)GameObjects.Worldmap_Line);
        _lr = _lrGO.GetComponent<UILineRenderer>();
        _arrow = Get<GameObject>((int)GameObjects.Worldmap_Minimap_Arrow);
        _arrow.SetActive(false);
        _minimapShip = Get<GameObject>((int)GameObjects.Worldmap_Minimap_Ship);
        _weatherText = Get<GameObject>((int)GameObjects.WeatherText);
        _movingText = Get<GameObject>((int)GameObjects.Worldmap_Minimap_MovingText);
        _timeText = Get<GameObject>((int)GameObjects.Worldmap_Minimap_TimeText);
        _timeText.SetActive(false);
        _distanceText = Get<GameObject>((int)GameObjects.Worldmap_Minimap_DistanceText);
        _distanceText.SetActive(false);

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
        _elapsedTime += Time.deltaTime;
        Vector2 direction = _target.transform.position - _ship.transform.position;
        if (_elapsedTime >= _timePerMeter)
        {
            _ship.transform.position += ((Vector3)(direction.normalized) * (_elapsedTime / _timePerMeter));
            _elapsedTime %= _timePerMeter;
        }
        SetLine();
        if (direction.x >= 0)
        {
            _ship.GetComponent<Image>().sprite = _rightShip;
            _minimapShip.GetComponent<Image>().sprite = _rightShip;
        }
        else
        {
            _ship.GetComponent<Image>().sprite = _leftShip;
            _minimapShip.GetComponent<Image>().sprite = _leftShip;
        }

        SetTimeDistanceText(direction);
        SetArrow(direction);
    }

    public void SetTimeDistanceText(Vector2 direction)//남은 시간, 남은 거리 갱신
    {
        _timeText.SetActive(true);
        _distanceText.SetActive(true);
        _timeText.GetComponent<TMP_Text>().SetText("남은 시간: " + (int)(direction.magnitude / _speed) + "sec");
        _distanceText.GetComponent<TMP_Text>().SetText("남은 거리: " + (int)(direction.magnitude) + "m");
    }

    public void SetArrow(Vector2 direction)//화살표 방향 갱신
    {
        _arrow.SetActive(true);
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);
        _arrow.transform.rotation = rot;
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
        _movingText.GetComponent<TMP_Text>().SetText("정지 중");
        _timeText.SetActive(false);
        _distanceText.SetActive(false);
        _arrow.SetActive(false);
    }

    public void OpenWorldmapUI(PointerEventData evt)
    {
        _worldmap.SetActive(true);
    }

    public void CloseWorldmapUI(PointerEventData evt)
    {
        _worldmap.SetActive(false);
    }

    /*
    public void GetMapPosition(Button btn)
    {
        Debug.Log(btn.name);
        Vector3[] temp = new Vector3[4];
        Vector3[] pos = new Vector3[2];

        btn.transform.GetChild(1).GetComponent<RectTransform>().GetWorldCorners(temp);
        pos[0] = temp[0];
        pos[1] = temp[2];
        Debug.Log(pos[0]);
        Debug.Log(pos[1]);
    }
    */
    
    public bool CheckWeather()//날씨 체크
    {
        for (int i = 1; i <= Managers.Data.WorldmapDict.Count; i++)
        {
            if (CheckPosition(i))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckPosition(int i)//특정 좌표에 들어왔는지 확인
    {
        if (_ship.transform.position.x >= Managers.Data.WorldmapDict[i].minX
            && _ship.transform.position.x <= Managers.Data.WorldmapDict[i].maxX
            && _ship.transform.position.y >= Managers.Data.WorldmapDict[i].minY
            && _ship.transform.position.y <= Managers.Data.WorldmapDict[i].maxY)
        {
            _weatherText.GetComponent<TMP_Text>().SetText(Managers.Data.WorldmapDict[i].name + " " + Managers.Data.WorldmapDict[i].weather);

            string weather = Managers.Data.WorldmapDict[i].weather;

            if(weather == "sunny")
            {
                Managers.Weather.UpdateWeather(EnumWeather.Sun);
            }
            else if(weather =="hot")
            {
                Managers.Weather.UpdateWeather(EnumWeather.Desert);
            }
            else if(weather =="rainy")
            {
                Managers.Weather.UpdateWeather(EnumWeather.Rain);
            }
            return true;
        }
        else
        {
            Managers.Weather.UpdateWeather(EnumWeather.Sun);
            return false;
        }
    }
}
