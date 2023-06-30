using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Worldmap_Button : MonoBehaviour
{
    private UI_Worldmap worldmap;

    private void Start()
    {
        worldmap = gameObject.transform.parent.GetComponent<UI_Worldmap>();
    }
    public void OnWorldmapButtonClick()
    {
        worldmap.setTarget(gameObject);
        Debug.Log(gameObject.name);
    }
}
