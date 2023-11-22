using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact_2 : Artifact
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
        Debug.Log("Artifact2 Select 메소드 사용됨");
    }

    public override void Deselect()
    {
        Debug.Log("Artifact2 Deselect 메소드 사용됨");
    }

    public override void Skill()
    {
        base.Skill();
    }
}
