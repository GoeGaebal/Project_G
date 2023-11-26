using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Artifact/Artifact_1")]

public class Artifact_1 : Artifact
{
    public override void Select()
    {
        base.Select();

        Debug.Log("Artifact1 Select 메소드 사용됨");
    }

    public override void Deselect()
    {
        base.Deselect();
        
        Debug.Log("Artifact1 Deselect 메소드 사용됨");
        Managers.Network.LocalPlayer.realDamage /= 1.1f;
    }

    public override void Skill()
    {
        base.Skill();

        Managers.Network.LocalPlayer.realDamage *= 1.1f;
    }
}
