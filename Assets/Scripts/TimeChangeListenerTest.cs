using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeChangeListenerTest : MonoBehaviour, ITimeSlotChangeEventListener
{
    public TimeSlotManager timeSlotManager;
    public PostProcessVolume postProcessVolume;

    private ColorGrading colorGrading;
    public PostProcessProfile profile;
    // Start is called before the first frame update
    void Start()
    {
        this.AssignEventHandler();
        this.postProcessVolume = GetComponent<PostProcessVolume>();

        profile.TryGetSettings(out colorGrading);
    }   



    public void TimeSlotChangeEventHandler(EnumTimeSlot timeSlot)
    {
        switch(timeSlot)
        {
            case EnumTimeSlot.Day:
                this.colorGrading.colorFilter.value = new Color(1.0f, 1.0f, 1.0f,1.0f);
                Debug.Log("day time test");
                break;
            case EnumTimeSlot.Night:
                this.colorGrading.colorFilter.value = new Color(0.243592f, 0.2473907f, 0.3018868f,1.0f);
                Debug.Log("night time test");
                break;
        }
    }

    public void AssignEventHandler()
    {
        timeSlotManager.AddListener(this);
    }
}
