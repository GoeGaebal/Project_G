using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactTile : MonoBehaviour, IInteractable
{
    public SpriteRenderer image;

    void Start()
    {
        image = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        Managers.Artifact.SetCurrentIndex(transform.parent.name[8] - 48);
        UI_Artifact.open();
    }
}
