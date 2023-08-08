using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
     [SerializeField]
    private Material m_pPPMaterial = null;

    [SerializeField, Range(0, 1)]
    float _Gray = 1;


    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (m_pPPMaterial == null) return;

        //m_pPPMaterial.SetFloat("_Gray", _Gray);     //! 쉐이더 Properties값 조작

        Graphics.Blit(src, dest, m_pPPMaterial);    //! 쉐이더 적용
    }
}
