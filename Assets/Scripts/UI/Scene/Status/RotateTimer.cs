using UnityEngine;
using System;

public class RotateTimer : MonoBehaviour
{
    [SerializeField] private GameObject rotatingTimer;
    private float ratio = 0;

    private float timerAngleOffset = 45.0f;
    public static Func<Quaternion> GetTimerAngle;
    public static Action<Quaternion> SetTimerAngle;
    void Start()
    {
        GetTimerAngle = () => { return transform.rotation; };
        SetTimerAngle = (q) => { _SetTimerAngle(q); };
        ratio = 360f / (Managers.TimeSlot.TimeChangePeriod * 2); // timeperiod * 2 초 동안 360' 돌아가야 함. 1초에 몇 도 돌아가야 하는지.

        _SetTimerAngle(Quaternion.Euler(0f, 0f, timerAngleOffset + (Managers.TimeSlot.CurrentTime % Managers.TimeSlot.TimeChangePeriod) * ratio));
    }   

    // Update is called once per frame
    void FixedUpdate()
    {
        AddTime();
    }

    private void AddTime()
    {
        rotatingTimer.transform.Rotate( new Vector3(0f, 0f, ratio * Time.fixedDeltaTime));
    }

    private Quaternion _GetTimerAngle()
    {
        return transform.rotation;
    }

    private void _SetTimerAngle(Quaternion rotateAngle)
    {
        rotatingTimer.transform.rotation = rotateAngle;
    }
}