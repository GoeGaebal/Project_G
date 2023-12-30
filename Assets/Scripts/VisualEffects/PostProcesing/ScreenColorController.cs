using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenColorController : MonoBehaviour, IWeatherChangeEventListener, ITimeSlotChangeEventListener
{
    [SerializeField]private Volume volume;
    

    [SerializeField]private VolumeParameter<Color> defaultColor;
    [SerializeField]private VolumeParameter<Color> sunDayColor;
    [SerializeField]private VolumeParameter<Color> sunNightColor;
    [SerializeField]private VolumeParameter<Color> desertDayColor;
    [SerializeField]private VolumeParameter<Color> desertNightColor;
    [SerializeField]private VolumeParameter<Color> rainDayColor;
    [SerializeField]private VolumeParameter<Color> rainNightColor;
    private ColorAdjustments colorAdjustments;
 
    void Start()
    {
        Managers.Weather.AddListener(this);
        Managers.TimeSlot.AddListener(this);
      

        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);

        UpdateScreenColor(Managers.TimeSlot.TimeSlot, Managers.Weather.Weather);
    }   



    public void WeatherChangeEventHandler(EnumWeather weather)
    {
        UpdateScreenColor(Managers.TimeSlot.TimeSlot, weather);
    }

       public void TimeSlotChangeEventHandler(EnumTimeSlot timeSlot)
    {
        UpdateScreenColor(timeSlot, Managers.Weather.Weather);
    }


    private void UpdateScreenColor(EnumTimeSlot timeSlot, EnumWeather weather)
    {
        switch(timeSlot)
        {
            case EnumTimeSlot.Day:
                if(weather == EnumWeather.Sun)
                    this.colorAdjustments.colorFilter.SetValue(this.sunDayColor);
                else if(weather == EnumWeather.Rain)
                    this.colorAdjustments.colorFilter.SetValue(this.rainDayColor);
                else if(weather == EnumWeather.Desert)
                    this.colorAdjustments.colorFilter.SetValue(this.desertDayColor);
                break;
            case EnumTimeSlot.Night:
                if(weather == EnumWeather.Sun)
                    this.colorAdjustments.colorFilter.SetValue(this.sunNightColor);
                else if(weather == EnumWeather.Rain)
                    this.colorAdjustments.colorFilter.SetValue(this.rainNightColor);
                else if(weather == EnumWeather.Desert)
                   this.colorAdjustments.colorFilter.SetValue(this.desertNightColor);
                break;
        }
    }
}
