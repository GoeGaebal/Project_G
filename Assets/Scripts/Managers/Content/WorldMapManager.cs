using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    private GameObject _ship;
    private GameObject _finalBoss;
    private float _finalBossMoveSpeed = 30.0f;
    private Vector3 _shipPosition;

    private bool _isBossBattle;
    private float _minDistacnceBetShipBoss;
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

    public void Init()
    {
        _isBossBattle = false;
        _minDistacnceBetShipBoss = 5.0f;
    }

    public void UpdateWorldMap(float deltaTime)
    {
        if(_isBossBattle) return;
        UpdateShipPosition();
        UpdateFinalBossPosition(deltaTime);
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
            Managers.Scene.LoadScene(Define.Scene.FinalBoss);
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
            }

            else
            {
                Managers.Weather.UpdateWeather(EnumWeather.Sun);

            }
 
        }   
    }
}

