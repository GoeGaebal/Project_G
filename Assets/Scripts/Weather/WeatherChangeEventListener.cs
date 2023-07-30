using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WeatherChangeEventListener : MonoBehaviour, IWeatherChangeEventListener
{
    private ColorGrading colorGrading;
    [SerializeField]private PostProcessProfile profile;

    [SerializeField]private Color sunColor;
    [SerializeField]private Color desertColor;
    [SerializeField]private Color rainColor;
    // Start is called before the first frame update

 
    void Start()
    {
         Managers.Weather.AddListener(this);
      

        profile.TryGetSettings(out colorGrading);

        this.colorGrading.colorFilter.value = this.sunColor;
    }   



    public void WeatherChangeEventHandler(EnumWeather weather)
    {
        switch(weather)
        {
            case EnumWeather.Sun:
                this.colorGrading.colorFilter.value = this.sunColor;
                break;
            case EnumWeather.Rain:
                this.colorGrading.colorFilter.value = this.rainColor;
                break;
            case EnumWeather.Desert:
                this.colorGrading.colorFilter.value = this.desertColor;
                break;
            default:
                this.colorGrading.colorFilter.value = this.sunColor;
                break;
        }
    }
}
