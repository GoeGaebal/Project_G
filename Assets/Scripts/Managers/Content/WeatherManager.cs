using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumWeather
{
    Sun,
    Desert,
    Rain
}

public class WeatherManager
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
                    Debug.Log("Weather changed Sun");
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
}
