using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public const int INT_SIZE_IN_BITS = sizeof(int) * 8;
    
    public enum Scene
    {
        Unknown,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
        BeginDrag,
        EndDrag,
        Drop,
    }

    public enum RaiseEventCode
    {
        RequestObjectInfos,
        ReceiveObjectInfos,
        RequestViewID,
        ReceiveViewID,
        ReceiveChat,
    }
}
