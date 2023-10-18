using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class WorldMapManager
{
    private GameObject _ship;
    private GameObject _finalBoss;
    private float _finalBossMoveSpeed = 10.0f;
    private Vector3 _shipPosition;

    private bool _isBossBattle;
    private float _minDistacnceBetShipBoss;

    public string currentMapName = "Map_001";

    public GameObject Ship
    {
        get { return _ship; }
        set { _ship = value; }
    }

    public GameObject FinalBoss
    {
        get{ return _finalBoss; }
        set{ _finalBoss = value;}
    }

    public UI_Worldmap UI { get; set; }

    public void Init()
    {
        _isBossBattle = false;
        _minDistacnceBetShipBoss = 5.0f;
    }

    public void UpdateWorldMap(float deltaTime)
    {
        if(_isBossBattle) return;
        if(UI == null) return;
        // TODO : 추후 Server로 로직을 옮김
        if (Managers.Network.isHost)
        {
            UpdateShipPosition();
            UpdateFinalBossPosition(deltaTime);
            S_WorldMap worldMap = new S_WorldMap();
            worldMap.ShipPosX = _shipPosition.x;
            worldMap.ShipPosY = _shipPosition.y;
            worldMap.FinalBossX = _finalBoss.transform.localPosition.x;
            worldMap.FinalBossY = _finalBoss.transform.localPosition.y;
            Managers.Network.Server.Room.Broadcast(worldMap);
        }
    }

    public void UpdateByPacket(S_WorldMap worldMap)
    {
        if (UI == null) return;
        _shipPosition.x = worldMap.ShipPosX;
        _shipPosition.y = worldMap.ShipPosY;
        _finalBoss.transform.localPosition = new Vector3(worldMap.FinalBossX, worldMap.FinalBossY);
    }
    private void UpdateShipPosition()
    {
        if(_ship == null)
        {
            return;
        }
        _shipPosition = _ship.transform.localPosition;
        CheckWeather();
    }
    private void UpdateFinalBossPosition(float deltaTime)
    {
        if(_finalBoss == null || _shipPosition == null)
            return;
        Vector3 direction = (_shipPosition - _finalBoss.transform.localPosition).normalized;

        _finalBoss.transform.localPosition = _finalBoss.transform.localPosition + direction * _finalBossMoveSpeed * deltaTime;

        if((_shipPosition - _finalBoss.transform.localPosition).sqrMagnitude < _minDistacnceBetShipBoss)
        {
            Debug.Log("final boss");
            _isBossBattle = true;
            Managers.Network.Server.Room.LoadScene(SceneType.FinalBoss);
        }
    }


    public void CheckWeather()//날씨 체크
    {
        for (int i = 1; i <= Managers.Data.WorldmapDict.Count; i++)
        {
            if (_shipPosition.x >= Managers.Data.WorldmapDict[i].minX
            && _shipPosition.x <= Managers.Data.WorldmapDict[i].maxX
            && _shipPosition.y >= Managers.Data.WorldmapDict[i].minY
            && _shipPosition.y <= Managers.Data.WorldmapDict[i].maxY)
            {

                string weather = Managers.Data.WorldmapDict[i].weather;


                if(weather == "sunny" && Managers.Weather.Weather != EnumWeather.Sun)
                {
                    Managers.Weather.UpdateWeather(EnumWeather.Sun);
                }
                else if(weather =="hot" && Managers.Weather.Weather != EnumWeather.Desert)
                {
                    Managers.Weather.UpdateWeather(EnumWeather.Desert);
                }
                else if(weather =="rainy" && Managers.Weather.Weather != EnumWeather.Rain)
                {
                    Managers.Weather.UpdateWeather(EnumWeather.Rain);
                }
            }

        }   
    }
}

