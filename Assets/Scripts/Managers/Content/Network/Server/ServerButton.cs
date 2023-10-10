using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ServerButton : MonoBehaviour
{
    public void Begin()
    {
        Managers.Network.Server.Init();
        Managers.Network.isHost = true;
    }
}
