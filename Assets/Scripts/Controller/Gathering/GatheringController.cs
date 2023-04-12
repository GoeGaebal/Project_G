using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEditor;

public class GatheringController : DamageableEntity
{
    // TODO : 피격하며 죽을 시 아이템 뿌리기
    private void Start()
    {
        base.dieAction += () =>
        {
            
        };
    }
}
