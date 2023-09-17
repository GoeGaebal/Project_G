using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        UI_Worldmap.open();
    }
}
