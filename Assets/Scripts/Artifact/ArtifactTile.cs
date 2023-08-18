using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactTile : MonoBehaviour, IInteractable
{
    private Sprite image;

    void Start()
    {
        image = GetComponent<SpriteRenderer>().sprite;
    }

    public void Interact()
    {
        Debug.Log("유물" + name + "상호작용됨");
        UI_Artifact.open();
    }
}
