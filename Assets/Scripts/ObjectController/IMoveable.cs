using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public interface IMoveable
{ 
    void OnRun(CreatureState state);
}
