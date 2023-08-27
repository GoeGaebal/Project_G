using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public enum EnumWeather
{
    Sun,
    Desert,
    Rain
}

public class WeatherManager : IOnEventCallback
{

    private float _curremtTime;
    private EnumWeather _weather = EnumWeather.Sun;

    public EnumWeather Weather
    {
        get{
            return _weather;
        }
        set{
            _weather = value;
        }
    }

    
    public delegate void _weatherChangeDel(EnumWeather weather);

    public event _weatherChangeDel WeatherChangeEvent;

    public void Init()
    {

        PhotonNetwork.AddCallbackTarget(this);

        if(!PhotonNetwork.IsMasterClient) Managers.Network.RequestSynchronizeTime();
         
    }


    private void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void AddListener(IWeatherChangeEventListener weatherChangeEventListener) 
    {
        Debug.Log("addListener");

        
        this.WeatherChangeEvent += weatherChangeEventListener.WeatherChangeEventHandler;
    }

    public void RemoveListener(IWeatherChangeEventListener weatherChangeEventListener)
    {
        this.WeatherChangeEvent -= weatherChangeEventListener.WeatherChangeEventHandler;
    }


    public void UpdateWeather(EnumWeather changeWeather)
    {
        
       switch(changeWeather)
            {
                case EnumWeather.Sun:
                    //Debug.Log("Weather changed Sun");
                    Weather = EnumWeather.Sun;
                    if(WeatherChangeEvent!= null)
                        WeatherChangeEvent(changeWeather);
                    break;
                case EnumWeather.Desert:
                    Debug.Log("Weather changed Desert");
                    Weather = EnumWeather.Desert;
                    if(WeatherChangeEvent!= null)
                        WeatherChangeEvent(changeWeather);
                    break;
                case EnumWeather.Rain:
                    Debug.Log("Weather changed Rain");
                    Weather = EnumWeather.Rain;
                    if(WeatherChangeEvent!= null)
                        WeatherChangeEvent(changeWeather);
                    break;
                default:
                    break;
            }
    }

    // 서버 동기화 관련. 일단 보류
    public void OnEvent(EventData photonEvent)
    {

    //     byte eventCode = photonEvent.Code;

    //     switch (eventCode)

    //     {
    //         case (byte)NetworkManager.CustomRaiseEventCode.SynchronizeTime:
    //             if (!PhotonNetwork.IsMasterClient)
    //             {
    //                 Debug.Log("recieved time event");
    //                 object[] data = (object[])photonEvent.CustomData;
                    
    //                 CurrentTime = (float)data[0];
    //                 TimeSlot = (EnumTimeSlot)data[1];
    //                 CountTimeSlotChanged = (int)data[2];
    //                 RotateTimer.SetTimerAngle((Quaternion)data[3]);
    //             }
    //             UpdateTimeSlot(TimeSlot);
    //             break;
    //         case (byte)NetworkManager.CustomRaiseEventCode.RequestSynchronizeTime:
    //             if(!PhotonNetwork.IsMasterClient) break;
    //             Managers.Network.SynchronizeTime();
    //             break;
    //         default:
    //             break;
    //     }
        
    }
}
