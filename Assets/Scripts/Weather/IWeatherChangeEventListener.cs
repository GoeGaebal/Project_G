using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeatherChangeEventListener 
{
    public void WeatherChangeEventHandler(EnumWeather weather);
}
