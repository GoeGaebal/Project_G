using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClientButton : MonoBehaviour
{
    public void Begin()
    {
        Managers.Network.Client.Init();
    }
}
