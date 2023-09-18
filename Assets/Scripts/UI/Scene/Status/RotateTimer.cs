using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class RotateTimer : MonoBehaviour
{
    [SerializeField] private GameObject rotatingTimer;
    private float ratio = 0;

    public static Func<Quaternion> GetTimerAngle;
    public static Action<Quaternion> SetTimerAngle;
    void Start()
    {
        GetTimerAngle = () => { return transform.rotation; };
        SetTimerAngle = (q) => { _SetTimerAngle(q); };
        ratio = Time.fixedDeltaTime / Managers.TimeSlot.TimeChangePeriod;

        _SetTimerAngle(Quaternion.Euler(0f, 0f, 45f + (Managers.TimeSlot.CurrentTime % Managers.TimeSlot.TimeChangePeriod) * ratio));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AddTime();
    }

    private void AddTime()
    {
        rotatingTimer.transform.rotation *= Quaternion.Euler(0f, 0f, 180f * ratio);
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