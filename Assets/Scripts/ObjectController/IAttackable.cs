using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public interface IAttackable
{
    void OnAttack(CreatureState prevState);
}
