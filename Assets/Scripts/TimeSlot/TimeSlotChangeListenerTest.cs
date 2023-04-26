using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeSlotChangeListenerTest : MonoBehaviour, ITimeSlotChangeEventListener
{
    private ColorGrading colorGrading;
    [SerializeField]private PostProcessProfile profile;

    [SerializeField]private Color dayTimeColor;
    [SerializeField]private Color nightTimeColor;
    // Start is called before the first frame update

 
    void Start()
    {
         Managers.TimeSlot.AddListener(this);
      

        profile.TryGetSettings(out colorGrading);
    }   



    public void TimeSlotChangeEventHandler(EnumTimeSlot timeSlot)
    {
        switch(timeSlot)
        {
            case EnumTimeSlot.Day:
                this.colorGrading.colorFilter.value = this.dayTimeColor;
                Debug.Log("day time test");
                break;
            case EnumTimeSlot.Night:
                this.colorGrading.colorFilter.value = this.nightTimeColor;
                Debug.Log("night time test");
                break;
        }
    }

   
    
}
