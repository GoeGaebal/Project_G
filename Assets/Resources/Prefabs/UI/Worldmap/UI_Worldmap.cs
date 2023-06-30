using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UI_Worldmap : MonoBehaviour
{
    private GameObject ship;
    private GameObject lrGO;
    UILineRenderer lr;
    private Vector3 curPos = new();
    private GameObject target;
    
    void Start()
    {
        lrGO = transform.GetChild(0).gameObject;
        lr = lrGO.GetComponent<UILineRenderer>();
        ship = transform.GetChild(1).gameObject;
        target = ship;
    }

    void Update()
    {
        setLine();
    }
    
    public void setTarget(GameObject t)
    {
        target = t;
    }

    private void setLine()
    {
        lrGO.transform.position = ship.transform.position;
        curPos = ship.transform.InverseTransformPoint(ship.transform.position);
        lr.Points[0] = curPos;
        lr.Points[1] = ship.transform.InverseTransformPoint(target.transform.position);
        lr.SetAllDirty();
    }
}
