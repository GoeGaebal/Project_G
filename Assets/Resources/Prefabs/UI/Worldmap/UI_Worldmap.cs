using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UI_Worldmap : MonoBehaviour
{
    private GameObject ship;
    private GameObject target;
    private Vector3 curPos = new();
    private GameObject lrGO;
    UILineRenderer lr;

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

    private void FixedUpdate()
    {
        if(Vector2.Distance(ship.transform.position, target.transform.position) >= 1f)
        {
            moveToTarget();
        }
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

    private void moveToTarget()
    {
        float distance = Vector2.Distance(ship.transform.position, target.transform.position);
        Vector2 direction = target.transform.position - ship.transform.position;
        ship.transform.position += (Vector3)(direction / distance);
    }
}
