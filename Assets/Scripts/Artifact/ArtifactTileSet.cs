using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactTileSet : MonoBehaviour
{
    public ArtifactTile[] tiles = new ArtifactTile[3];
    public static int[] equippedArray = { -1, -1, -1};
    public static System.Action<int, Sprite> setImage;
    public static System.Action<int> resetImage;

    private void Awake()
    {
        setImage = (idx, image) => { SetTileImage(idx, image); };
        resetImage = (idx) => { ResetTileImage(idx); };
    }

    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            tiles[i] = transform.GetChild(i).GetChild(0).GetComponent<ArtifactTile>();
        }
    }

    public void SetTileImage(int idx, Sprite sprt = null)
    {
        tiles[idx].image.sprite = sprt;
    }

    public void ResetTileImage(int idx)
    {
        tiles[idx].image.sprite = null;
    }
}
