using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumTimeSlot
{
    Day, Night
};


public class TimeSlotManager
{
    private float timeChangePeriod;
    private float _curremtTime;
    private EnumTimeSlot _timeSlot = EnumTimeSlot.Day;
    public float CurrentTime{
        get{
            return _curremtTime;
        }
        private set{
            _curremtTime = value;
        }
    }

    public EnumTimeSlot TimeSlot
    {
        get{
            return _timeSlot;
        }
        private set{
            _timeSlot = value;
        }
    }

    
    public delegate void _timeSlotChangeDel(EnumTimeSlot time);

    public event _timeSlotChangeDel TimeSlotChangeEvent;

    public void Init()
    {
        CurrentTime = 0f;
        TimeSlot = EnumTimeSlot.Day;
    }
    public void AddListener(ITimeSlotChangeEventListener timeChangeEventListener) 
    {
        Debug.Log("addListener");
        this.TimeSlotChangeEvent += timeChangeEventListener.TimeSlotChangeEventHandler;
    }

    public void RemoveListener(ITimeSlotChangeEventListener timeChangeEventListener)
    {
        this.TimeSlotChangeEvent -= timeChangeEventListener.TimeSlotChangeEventHandler;
    }

    public void AddDelataTime(float deltaTime)
    {
        CurrentTime+=Time.deltaTime;
        if(CurrentTime >= timeChangePeriod)
        {
            this.CurrentTime = 0;
            switch(TimeSlot)
            {
                case EnumTimeSlot.Day:
                    Debug.Log("to night time manager changed");
                    TimeSlot = EnumTimeSlot.Night;
                    TimeSlotChangeEvent(this.TimeSlot);
                    break;
                case EnumTimeSlot.Night:
                    Debug.Log("to day night time manager changed");
                    TimeSlot = EnumTimeSlot.Day;
                    TimeSlotChangeEvent(this.TimeSlot);
                    break;
                default:
                    break;
            }
           
        }
    }
}
