using System;
using UnityEngine;

public class Map005 : MonoBehaviour
{
    [SerializeField] private GameObject lab;
    [SerializeField] private GameObject labDeco;
    [SerializeField] private GameObject labShadow;
    [SerializeField] private GameObject labOut;
    [SerializeField] private GameObject titles;
    [SerializeField] private GameObject keymaps;
    [SerializeField] private GameObject shadow;
    [SerializeField] private GameObject fence;
    private void FixedUpdate()
    {
        if (Managers.Network.LocalPlayer != null)
        {
            bool isOut = Managers.Network.LocalPlayer.transform.position.x > 25;
            if(labOut.activeSelf != isOut) labOut.SetActive(isOut);
            if(shadow.activeSelf != isOut) shadow.SetActive(isOut);
            if(fence.activeSelf != isOut) fence.SetActive(isOut);
            if(titles.activeSelf == isOut) titles.SetActive(!isOut);
            if(keymaps.activeSelf == isOut) keymaps.SetActive(!isOut);
            if(lab.activeSelf == isOut) lab.SetActive(!isOut);
            if(labDeco.activeSelf == isOut) labDeco.SetActive(!isOut);
            if(labShadow.activeSelf == isOut) labShadow.SetActive(!isOut);
        }
    }
}
