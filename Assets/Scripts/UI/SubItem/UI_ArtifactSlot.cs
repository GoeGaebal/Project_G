using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_ArtifactSlot : UI_Base, IPointerClickHandler
{
    enum Images
    {
        ArtifactImage,
    }

    [HideInInspector] public Artifact artifact;
    [HideInInspector] private Image _image;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));

        _image = Get<Image>((int)Images.ArtifactImage);
        _image.sprite = artifact.Image;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (artifact.name == "Artifact_Deselect")
        {
            Managers.Artifact.DeselectArtifact();
            Debug.Log("유물 해제됨");
            UI_Artifact.close();
        }
        else
        {
            Managers.Artifact.SelectArtifact(artifact);
            Debug.Log(artifact.name + "유물 선택됨");
            UI_Artifact.close();
        }
    }
}
