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

    enum GameObjects
    {
        Equipped,
    }

    [HideInInspector] public Artifact artifact;
    private bool isEquipped;
    private Image _image;
    private GameObject _equippedImage;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        isEquipped = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        _image = Get<Image>((int)Images.ArtifactImage);
        _image.sprite = artifact.Image;

        _equippedImage = Get<GameObject>((int)GameObjects.Equipped);
        _equippedImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (artifact.name == "Artifact_Deselect")
        {
            Managers.Artifact.DeselectArtifact();
            Managers.Artifact.equippedArtifactSlots[Managers.Artifact.currentIndex].SetEquipped(false);
            //UI_Artifact.close();
        }
        else
        {
            if (!isEquipped)
            {
                if(Managers.Artifact.equippedArtifactSlots[Managers.Artifact.currentIndex] != null)
                {
                    Managers.Artifact.equippedArtifactSlots[Managers.Artifact.currentIndex].SetEquipped(false);
                }
                Managers.Artifact.equippedArtifactSlots[Managers.Artifact.currentIndex] = GetComponent<UI_ArtifactSlot>();
                Managers.Artifact.SelectArtifact(artifact);
                SetEquipped(true);
                UI_Artifact.close();
            }
        }
    }

    public void SetEquipped(bool b)
    {
        isEquipped = b;
        _equippedImage.SetActive(b);
    }
}
