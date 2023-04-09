using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChangeListenerTest : MonoBehaviour, ITimeSlotChangeEventListener
{
    public TimeSlotManager timeSlotManager;
    // Start is called before the first frame update
    void Start()
    {
        this.AssignEventHandler();
    }



    public void TimeSlotChangeEventHandler(EnumTimeSlot timeSlot)
    {
        switch(timeSlot)
        {
            case EnumTimeSlot.Day:
                Debug.Log("day time test");
                break;
            case EnumTimeSlot.Night:
                Debug.Log("night time test");
                break;
        }
    }

    public void AssignEventHandler()
    {
        timeSlotManager.AddListener(this);
    }
}
