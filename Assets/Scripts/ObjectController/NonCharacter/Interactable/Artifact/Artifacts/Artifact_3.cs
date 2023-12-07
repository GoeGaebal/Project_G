using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Artifact/Artifact_3")]

public class Artifact_3 : Artifact
{
    public override void Select()
    {
        base.Select();
        Debug.Log("Artifact3 Select 메소드 사용됨");
    }

    public override void Deselect()
    {
        base.Deselect();
        Debug.Log("Artifact3 Deselect 메소드 사용됨");

        Managers.Network.LocalPlayer.damageMultiply -= DamageMul;
    }

    public override void Skill()
    {
        base.Skill();

        Managers.Network.LocalPlayer.damageMultiply += DamageMul;
    }
}
