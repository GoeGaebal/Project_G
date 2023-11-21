using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorPostProcessing : MonoBehaviour, IWeatherChangeEventListener, ITimeSlotChangeEventListener
{
    [SerializeField]private Volume volume;
    [SerializeField]private GameObject rainSystem;
    [SerializeField]private GameObject rainSystemPrefab;
    

    [SerializeField]private VolumeParameter<Color> defaultColor;
    [SerializeField]private VolumeParameter<Color> sunDayColor;
    [SerializeField]private VolumeParameter<Color> sunNightColor;
    [SerializeField]private VolumeParameter<Color> desertDayColor;
    [SerializeField]private VolumeParameter<Color> desertNightColor;
    [SerializeField]private VolumeParameter<Color> rainDayColor;
    [SerializeField]private VolumeParameter<Color> rainNightColor;
    [SerializeField]private ScriptableRendererFeature _universalRenderfeature;
    private ColorAdjustments colorAdjustments;
 
    void Start()
    {
        Managers.Weather.AddListener(this);
      
        Managers.TimeSlot.AddListener(this);
      

        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);

       switch(Managers.Weather.Weather)
        {
            case EnumWeather.Sun:
                rainSystem?.SetActive(false);
                _universalRenderfeature.SetActive(false);
                if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Day)
                    this.colorAdjustments.colorFilter.SetValue(this.sunDayColor);
                else if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Night)
                    this.colorAdjustments.colorFilter.SetValue(this.sunNightColor);
                break;
            case EnumWeather.Rain:
                rainSystem.SetActive(true);
                 _universalRenderfeature.SetActive(false);
                if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Day)
                    this.colorAdjustments.colorFilter.SetValue(this.rainDayColor);
                else if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Night)
                    this.colorAdjustments.colorFilter.SetValue(this.rainNightColor);
                break;
            case EnumWeather.Desert:
                rainSystem?.SetActive(false);
                _universalRenderfeature.SetActive(true);
                if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Day)
                    this.colorAdjustments.colorFilter.SetValue(this.desertDayColor);
                else if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Night)
                    this.colorAdjustments.colorFilter.SetValue(this.desertNightColor);
                break;
            default:
                this.colorAdjustments.colorFilter.SetValue(this.sunDayColor);
                break;
        }
    }   



    public void WeatherChangeEventHandler(EnumWeather weather)
    {
        if(rainSystem == null)
        {
            rainSystem = Instantiate<GameObject>(rainSystemPrefab);
        }
        switch(weather)
        {
            case EnumWeather.Sun:
                rainSystem?.SetActive(false);
                _universalRenderfeature.SetActive(false);
                if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Day)
                    this.colorAdjustments.colorFilter.SetValue(this.sunDayColor);
                else if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Night)
                    this.colorAdjustments.colorFilter.SetValue(this.sunNightColor);
                break;
            case EnumWeather.Rain:
                rainSystem.SetActive(true);
                _universalRenderfeature.SetActive(false);
                if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Day)
                    this.colorAdjustments.colorFilter.SetValue(this.rainDayColor);
                else if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Night)
                    this.colorAdjustments.colorFilter.SetValue(this.rainNightColor);
                break;
            case EnumWeather.Desert:
                rainSystem?.SetActive(false);
                _universalRenderfeature.SetActive(true);
                if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Day)
                    this.colorAdjustments.colorFilter.SetValue(this.desertDayColor);
                else if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Night)
                    this.colorAdjustments.colorFilter.SetValue(this.desertNightColor);
                break;
            default:
                this.colorAdjustments.colorFilter.SetValue(this.sunDayColor);
                break;
        }
    }

       public void TimeSlotChangeEventHandler(EnumTimeSlot timeSlot)
    {
        Debug.Log("timeSlotChangeEventHandler");
        switch(timeSlot)
        {
            case EnumTimeSlot.Day:
                if(Managers.Weather.Weather == EnumWeather.Sun)
                    this.colorAdjustments.colorFilter.SetValue(this.sunDayColor);
                else if(Managers.Weather.Weather == EnumWeather.Rain)
                    this.colorAdjustments.colorFilter.SetValue(this.rainDayColor);
                else if(Managers.Weather.Weather == EnumWeather.Desert)
                    this.colorAdjustments.colorFilter.SetValue(this.desertDayColor);
                break;
            case EnumTimeSlot.Night:
                if(Managers.Weather.Weather == EnumWeather.Sun)
                    this.colorAdjustments.colorFilter.SetValue(this.sunNightColor);
                else if(Managers.Weather.Weather == EnumWeather.Rain)
                    this.colorAdjustments.colorFilter.SetValue(this.rainNightColor);
                else if(Managers.Weather.Weather == EnumWeather.Desert)
                   this.colorAdjustments.colorFilter.SetValue(this.desertNightColor);
                break;
        }
    }
}
