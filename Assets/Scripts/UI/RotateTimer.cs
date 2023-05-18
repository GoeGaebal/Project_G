using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateTimer : MonoBehaviour
{
    [SerializeField] private GameObject rotatingTimer;
    private float ratio = 0;

    // Start is called before the first frame update
    void Start()
    {
        ratio = Time.deltaTime / Managers.TimeSlot.timeChangePeriod;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AddTime();
    }
    
    private void AddTime()
    {
        rotatingTimer.transform.rotation *= Quaternion.Euler(0f, 0f, 360f * ratio);
    }
}
