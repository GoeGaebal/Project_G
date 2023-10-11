using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class UI_Worldmap : UI_Scene
{
    //enum Images { Worldmap_Background }

    enum GameObjects {
        Worldmap_Background,
        Worldmap_Ship,
        Worldmap_Line,
        //WeatherText,
        Worldmap_Minimap_Background,
        Worldmap_Minimap_Arrow,
        Worldmap_Minimap_Ship,
        Worldmap_Minimap_MovingText,
        Worldmap_Minimap_TimeText,
        Worldmap_Minimap_DistanceText,
        Worldmap_FinalBoss
    }
    enum Buttons
    {
        //Worldmap_Button,
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
    private Vector3 _targetPosition;//이동 목표
    private float _elapsedTime = 0f;
    private int _speed = 50;//초당 이동 거리
    private float _timePerMeter;//거리 1 이동하는데 걸리는 시간
    private GameObject _lrGO;
    private GameObject _worldmap;
    private UILineRenderer _lr;
    //private GameObject _weatherText;
    private GameObject _arrow;
    private GameObject _minimapShip;
    private GameObject _movingText;
    private GameObject _timeText;
    private GameObject _distanceText;
    private GameObject _finalBoss;

    private bool _moveFlag = false;//이동 중
    private bool _arriveFlag = false;//도착

    private Portal _portal;

    private string _mapName;

    public static System.Action open;

    private void Awake()
    {
        open = () => { OpenWorldmapUI(); };
    }

    void Start()
    {
        Init();
    }

    private void Update()
    {
        //UpdateWeatherUI();

        if (_moveFlag)
        {
            if (Vector2.Distance(_ship.transform.localPosition, _targetPosition) >= 1f)
            {
                MoveToTarget();
                _movingText.GetComponent<TMP_Text>().SetText(Managers.Data.WorldmapDict[_mapName[6] - '0'].name + "로 이동 중");
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
            _movingText.GetComponent<TMP_Text>().SetText(Managers.Data.WorldmapDict[_mapName[6] - '0'].name + "에 정박 중");
            //TODO: 필드로 이동 가능하게 하는 코드
            //_mapName 이용해서 해당하는 맵 프리팹을 필드에 생성시키기.

            _portal.SetColor(Color.blue);
            _portal.SetPortal(true);
        }
        else
        {
            _portal.SetColor(Color.gray);
            _portal.SetPortal(false);
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
        _targetPosition = _ship.transform.localPosition;
        _timePerMeter = 1f / _speed;
        _lrGO = Get<GameObject>((int)GameObjects.Worldmap_Line);
        _lr = _lrGO.GetComponent<UILineRenderer>();
        _arrow = Get<GameObject>((int)GameObjects.Worldmap_Minimap_Arrow);
        _arrow.SetActive(false);
        _minimapShip = Get<GameObject>((int)GameObjects.Worldmap_Minimap_Ship);
        //_weatherText = Get<GameObject>((int)GameObjects.WeatherText);
        _movingText = Get<GameObject>((int)GameObjects.Worldmap_Minimap_MovingText);
        _timeText = Get<GameObject>((int)GameObjects.Worldmap_Minimap_TimeText);
        _timeText.SetActive(false);
        _distanceText = Get<GameObject>((int)GameObjects.Worldmap_Minimap_DistanceText);
        _distanceText.SetActive(false);
        _finalBoss = Get<GameObject>((int)GameObjects.Worldmap_FinalBoss);

        _portal = GameObject.Find("Portal").GetComponent<Portal>();

        //GetButton((int)Buttons.Worldmap_Button).gameObject.BindEvent(OpenWorldmapUI);
        GetButton((int)Buttons.Worldmap_Button_Close).gameObject.BindEvent(CloseWorldmapUI);
        GetButton((int)Buttons.Map_001).gameObject.BindEvent(OnWorldmapButtonClick);
        GetButton((int)Buttons.Map_002).gameObject.BindEvent(OnWorldmapButtonClick);
        GetButton((int)Buttons.Map_003).gameObject.BindEvent(OnWorldmapButtonClick);
        GetButton((int)Buttons.Worldmap_Button_Stop).onClick.AddListener(PauseMove);

        //set the ship position on world map manager
        Managers.WorldMap.Ship = _ship;
        Managers.WorldMap.FinalBoss = _finalBoss;

        Managers.WorldMap.UI = this;
    }

    public void SetTarget(GameObject t)//목표 지정
    {
        _targetPosition = t.transform.localPosition;
        _mapName = t.name;
    }
    private void MoveToTarget()//목표 방향으로 이동
    {
        _elapsedTime += Time.deltaTime;
        Vector2 direction = _targetPosition - _ship.transform.localPosition;
        if (_elapsedTime >= _timePerMeter)
        {
            _ship.transform.localPosition = _ship.transform.localPosition + ((Vector3)(direction.normalized) * (_elapsedTime / _timePerMeter));
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
        _timeText.GetComponent<TMP_Text>().SetText("남은 시간: " + ((int)(direction.magnitude / _speed) + 1) + "sec");
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
        _lr.Points[0] = Vector2.zero;
        _lr.Points[1] = _targetPosition - _lrGO.transform.localPosition;
        _lr.SetAllDirty();
    }

    public void OnWorldmapButtonClick(PointerEventData evt)
    {
        if (!Managers.Network.isHost) return;
        
        SetTarget(evt.pointerPress);
        _arriveFlag = false;
        _moveFlag = true;
        _lr.enabled = true;
        
        S_WorldMapEvent packet = new S_WorldMapEvent();
        packet.Event = UIWorldMapEventType.SetTarget;
        packet.ShipPosX = _ship.transform.localPosition.x;
        packet.ShipPosY = _ship.transform.localPosition.y;
        packet.TargetPosX = _targetPosition.x;
        packet.TargetPosY = _targetPosition.y;
        packet.MapName = _mapName;
        Managers.Network.Server.Room.Broadcast(packet);
    }

    public void PauseMove()
    {
        if (!Managers.Network.isHost) return;
        
        SetTarget(_ship);
        _moveFlag = false;
        _lr.enabled = false;
        _movingText.GetComponent<TMP_Text>().SetText("정지 중");
        _timeText.SetActive(false);
        _distanceText.SetActive(false);
        _arrow.SetActive(false);

        S_WorldMapEvent packet = new S_WorldMapEvent();
        packet.Event = UIWorldMapEventType.PauseMove;
        packet.ShipPosX = _ship.transform.localPosition.x;
        packet.ShipPosY = _ship.transform.localPosition.y;
        packet.TargetPosX = _targetPosition.x;
        packet.TargetPosY = _targetPosition.y;
        packet.MapName = _mapName;
        Managers.Network.Server.Room.Broadcast(packet);
    }

    // TODO : 추후 통합 필요
    public void UpdateByPacket(S_WorldMapEvent evt)
    {
        if (evt.Event == UIWorldMapEventType.SetTarget)
        {
            _targetPosition = new Vector3(evt.TargetPosX, evt.TargetPosY);
            _ship.transform.localPosition = new Vector3(evt.ShipPosX, evt.ShipPosY);
            _mapName = evt.MapName;
            _arriveFlag = false;
            _moveFlag = true;
            _lr.enabled = true;
        }
        else
        {
            _targetPosition = new Vector3(evt.ShipPosX, evt.ShipPosY);
            _ship.transform.localPosition = new Vector3(evt.ShipPosX, evt.ShipPosY);
            _mapName = evt.MapName;
            _moveFlag = false;
            _lr.enabled = false;
            _movingText.GetComponent<TMP_Text>().SetText("정지 중");
            _timeText.SetActive(false);
            _distanceText.SetActive(false);
            _arrow.SetActive(false);
        }
    }

    public void OpenWorldmapUI(/*PointerEventData evt*/)
    {
        _worldmap.SetActive(true);
        Managers.Input.PlayerActionMap.Disable();
    }

    public void CloseWorldmapUI(PointerEventData evt)
    {
        _worldmap.SetActive(false);
        Managers.Input.PlayerActionMap.Enable();
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
    
    /*
    private void UpdateWeatherUI()
    {
        EnumWeather weather = Managers.Weather.Weather;

        switch (weather)
        {
            case EnumWeather.Sun:
                _weatherText.GetComponent<TMP_Text>().SetText("Sunny");
                break;
            case EnumWeather.Rain:
                _weatherText.GetComponent<TMP_Text>().SetText("Rain");
                break;
            case EnumWeather.Desert:
                _weatherText.GetComponent<TMP_Text>().SetText("Hot");
                break;
            default:
                _weatherText.GetComponent<TMP_Text>().SetText("Sunny");
                break;
        }
        
    }
    */
}
