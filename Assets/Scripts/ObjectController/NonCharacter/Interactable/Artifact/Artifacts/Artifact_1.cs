using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact_1 : Artifact
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Select()
    {
        Managers.Network.LocalPlayer.realDamage *= 1.1f;
        Debug.Log("Artifact1 Select 메소드 사용됨");
    }

    public override void Deselect()
    {
        Managers.Network.LocalPlayer.realDamage /= 1.1f;
        Debug.Log("Artifact1 Deselect 메소드 사용됨");
    }

    public override void Skill()
    {
        base.Skill();
    }
}
