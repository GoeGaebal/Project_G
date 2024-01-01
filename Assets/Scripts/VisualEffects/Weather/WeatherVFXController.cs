using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WeatherVFXController : MonoBehaviour, IWeatherChangeEventListener
{
    private GameObject rainSystem;
    [SerializeField]private ScriptableRendererFeature heatDistortionWeatherRenderfeature;

    void Awake()
    {
            rainSystem = transform.Find("RainSystem").gameObject;
    }
    

    void Start()
    {
        Managers.Weather.AddListener(this);
      
        UpdateWeather(Managers.Weather.Weather);
    }   



    public void WeatherChangeEventHandler(EnumWeather weather)
    {
       UpdateWeather(weather);
    }

    private void UpdateWeather(EnumWeather weather)
    {
        
        switch(weather)
        {
            case EnumWeather.Sun:
                rainSystem?.SetActive(false);
                heatDistortionWeatherRenderfeature?.SetActive(false);
                break;
            case EnumWeather.Rain:
                rainSystem?.SetActive(true);
                heatDistortionWeatherRenderfeature?.SetActive(false);
                break;
            case EnumWeather.Desert:
                rainSystem?.SetActive(false);
                heatDistortionWeatherRenderfeature?.SetActive(true);    
                break;
            default:
                rainSystem?.SetActive(false);
                heatDistortionWeatherRenderfeature?.SetActive(false);
                break;
        }
    }
}
