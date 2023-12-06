using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Artifact/Artifact_0")]

public class Artifact_0 : Artifact
{
    public override void Select()
    {
        base.Select();

        Debug.Log("아티팩트 장착 해제됨");
    }

    public override void Deselect()
    {
        base.Deselect();
    }

    public override void Skill()
    {
        base.Skill();
    }
}
