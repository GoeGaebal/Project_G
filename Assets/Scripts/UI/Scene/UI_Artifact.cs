using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Artifact : UI_Scene
{
    private GameObject _content;

    enum GameObjects
    {
        Content,
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    public override void Init()
    {
        //base.Init();
        Managers.UI.SetCanvas(gameObject, true);

        Bind<GameObject>(typeof(GameObjects));

        _content = Get<GameObject>((int)GameObjects.Content);

        Managers.Artifact.AddScroll("Artifact_1");
        Managers.Artifact.AddScroll("Artifact_2");

        for (int i = 0; i < Managers.Artifact.artifactScrolls.Length; i++)
        {
            var slot = Managers.Artifact.MakeArtifactSlot(_content.transform);
            slot.artifact = Managers.Artifact.artifactScrolls[i];
        }
    }
}