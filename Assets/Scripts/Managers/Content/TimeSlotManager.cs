using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumTimeSlot
{
    Day, Night
};


public class TimeSlotManager : MonoBehaviour
{
    private float timeChangePeriod = 5.0f;
    private float _curremtTime;
    private EnumTimeSlot _timeSlot;
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
    // Start is called before the first frame update
    void Start()
    {
        TimeSlot = EnumTimeSlot.Day;
        CurrentTime = 0f;
    }

    // Update is called once per frame
    void Update()
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

    public void AddListener(ITimeSlotChangeEventListener timeChangeEventListener) 
    {
        Debug.Log("addListener");
        this.TimeSlotChangeEvent += timeChangeEventListener.TimeSlotChangeEventHandler;
    }

    public void RemoveListener(ITimeSlotChangeEventListener timeChangeEventListener)
    {
        this.TimeSlotChangeEvent -= timeChangeEventListener.TimeSlotChangeEventHandler;
    }
}
