using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GasBossHeatArea : MonoBehaviour
{
    [SerializeField]private ScriptableRendererFeature _heatHazeDistortionRenderfeature;
    private void OnTriggerEnter2D(Collider2D other) {
        Player player = other.GetComponent<Player>();
        Debug.Log("보스 에어리어 인");
        if(Managers.Network.LocalPlayer == player) 
        {
            Debug.Log("보스 에어리어 로컬");
            _heatHazeDistortionRenderfeature.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        Player player = other.GetComponent<Player>();
        if(Managers.Network.LocalPlayer == player) 
        {
            _heatHazeDistortionRenderfeature.SetActive(false);
        }
    }
}
