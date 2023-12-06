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
        //Init();
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
        //_image.sprite = artifact.Image;

        _equippedImage = Get<GameObject>((int)GameObjects.Equipped);
        _equippedImage.SetActive(false);

        isEquipped = false;
    }

    public void SetArtifact(Artifact a)
    {
        artifact = a;
        _image.sprite = artifact.Image;
    }

    public void OnPointerClick(PointerEventData eventData)//아티팩트 클릭 시
    {
        if (artifact.name == "Artifact_0")
        {
            if(Managers.Artifact.artifacts[Managers.Artifact.currentIndex] != null)
            {
                Managers.Artifact.equippedArtifactSlots[Managers.Artifact.currentIndex].SetEquipped(false);
                Managers.Artifact.DeselectArtifact();
                //UI_Artifact.close();
            }
        }
        else
        {
            if (!isEquipped)//이미 장착 중인 아티팩트는 선택 불가
            {
                if(Managers.Artifact.equippedArtifactSlots[Managers.Artifact.currentIndex] != null)
                {//현재 타일셋에 장착된 다른 아티팩트가 있다면
                    Managers.Artifact.equippedArtifactSlots[Managers.Artifact.currentIndex].SetEquipped(false);//해제시킴
                }

                //내가 클릭한 아티팩트 장착
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
