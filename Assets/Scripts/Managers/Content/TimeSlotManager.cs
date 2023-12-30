using UnityEngine;

public enum EnumTimeSlot
{
    Day, Night
};


public class TimeSlotManager
{

    [SerializeField] private float _timeChangePeriod = 30.0f;
    public float TimeChangePeriod{
        get { return _timeChangePeriod;}
    }
    private float _curremtTime = 0f;
    private EnumTimeSlot _timeSlot = EnumTimeSlot.Day;
    private int countTimeSlotChanged = 1;
    public int CountTimeSlotChanged{
        get { return countTimeSlotChanged; }
        private set { countTimeSlotChanged = value; }
    }
    public float CurrentTime{
        get{
            return _curremtTime;
        }
        set{
            _curremtTime = value;
        }
    }
    
    public EnumTimeSlot TimeSlot
    {
        get{
            return _timeSlot;
        }
        set{
            _timeSlot = value;
        }
    }

    
    public delegate void _timeSlotChangeDel(EnumTimeSlot time);

    public event _timeSlotChangeDel TimeSlotChangeEvent;

    public void Init()
    {
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
        if(!ShipScene.isStarted) return;
        if(Managers.Scene.IsLoading) return;
        CurrentTime+=Time.deltaTime;
         if(CurrentTime >= _timeChangePeriod * countTimeSlotChanged && TimeSlotChangeEvent != null)
        {
            countTimeSlotChanged++;
            

            switch (TimeSlot)
            {
                
                case EnumTimeSlot.Day:
                    UpdateTimeSlot(EnumTimeSlot.Night);
                    break;
                case EnumTimeSlot.Night:
                    UpdateTimeSlot(EnumTimeSlot.Day);
                    break;
                default:
                    break; 
            }
            // Managers.Network.SynchronizeTime();
        }
    }

    private void UpdateTimeSlot(EnumTimeSlot changeTimeSlot)
    {
       switch(changeTimeSlot)
            {
                case EnumTimeSlot.Day:
                    Debug.Log("to night time manager changed");
                    TimeSlot = EnumTimeSlot.Day;
                    if(TimeSlotChangeEvent!= null)
                        TimeSlotChangeEvent(TimeSlot);
                    break;
                case EnumTimeSlot.Night:
                    Debug.Log("to day time manager changed");
                    TimeSlot = EnumTimeSlot.Night;
                    if(TimeSlotChangeEvent!= null)
                        TimeSlotChangeEvent(TimeSlot);
                    break;
                default:
                    break;
            }
    }

}
