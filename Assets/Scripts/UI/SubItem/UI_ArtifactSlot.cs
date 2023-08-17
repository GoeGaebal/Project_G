using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_ArtifactSlot : UI_Base, IPointerClickHandler
{
    enum Images
    {
        UI_ArtifactSlot
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

        _image = Get<Image>((int)Images.UI_ArtifactSlot);
        _image.sprite = artifact.Image;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
