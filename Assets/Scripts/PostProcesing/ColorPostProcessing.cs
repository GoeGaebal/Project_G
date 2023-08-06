using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ColorPostProcessing : MonoBehaviour, IWeatherChangeEventListener, ITimeSlotChangeEventListener
{
    private ColorGrading colorGrading;
    [SerializeField]private PostProcessProfile profile;

    [SerializeField]private Color defaultColor;
    [SerializeField]private Color sunDayColor;
    [SerializeField]private Color sunNightColor;
    [SerializeField]private Color desertDayColor;
    [SerializeField]private Color desertNightColor;
    [SerializeField]private Color rainDayColor;
    [SerializeField]private Color rainNightColor;
    // Start is called before the first frame update

 
    void Start()
    {
        Managers.Weather.AddListener(this);
      
        Managers.TimeSlot.AddListener(this);
      

        profile.TryGetSettings(out colorGrading);

        this.colorGrading.colorFilter.value = this.defaultColor;
    }   



    public void WeatherChangeEventHandler(EnumWeather weather)
    {
        switch(weather)
        {
            case EnumWeather.Sun:
                if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Day)
                    this.colorGrading.colorFilter.value = this.sunDayColor;
                else if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Night)
                    this.colorGrading.colorFilter.value = this.sunNightColor;
                break;
            case EnumWeather.Rain:
                if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Day)
                    this.colorGrading.colorFilter.value = this.rainDayColor;
                else if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Night)
                    this.colorGrading.colorFilter.value = this.rainNightColor;
                break;
            case EnumWeather.Desert:
                if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Day)
                    this.colorGrading.colorFilter.value = this.desertDayColor;
                else if(Managers.TimeSlot.TimeSlot == EnumTimeSlot.Night)
                    this.colorGrading.colorFilter.value = this.desertNightColor;
                break;
            default:
                this.colorGrading.colorFilter.value = this.sunDayColor;
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
                    this.colorGrading.colorFilter.value = this.sunDayColor;
                else if(Managers.Weather.Weather == EnumWeather.Rain)
                    this.colorGrading.colorFilter.value = this.rainDayColor;
                else if(Managers.Weather.Weather == EnumWeather.Desert)
                    this.colorGrading.colorFilter.value = this.desertDayColor;
                break;
            case EnumTimeSlot.Night:
                if(Managers.Weather.Weather == EnumWeather.Sun)
                    this.colorGrading.colorFilter.value = this.sunNightColor;
                else if(Managers.Weather.Weather == EnumWeather.Rain)
                    this.colorGrading.colorFilter.value = this.rainNightColor;
                else if(Managers.Weather.Weather == EnumWeather.Desert)
                    this.colorGrading.colorFilter.value = this.desertNightColor;
                break;
        }
    }
}
