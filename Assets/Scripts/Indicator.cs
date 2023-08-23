using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Indicator : MonoBehaviourPun
{

    [SerializeField] private GameObject indicatorPrefab;
    private  GameObject indicatorUICanvas;
    private static GameObject localPlayer;
    private GameObject indicatorUIInstance;
    private float defaultAngle;
 
 
    private void Start()
    {
   
        indicatorUIInstance = Instantiate(indicatorPrefab);

         RectTransform indicatorRect = indicatorUIInstance.GetComponent<RectTransform>();

        // TODO: Canvas를 가지는 첫 번째 오브젝트를 찾는다. 추후에 UI로 통합할 예정
        indicatorUICanvas = FindObjectOfType<Canvas>().gameObject;
        indicatorRect.SetParent(indicatorUICanvas.transform);
        indicatorUIInstance.transform.localScale = new Vector3(1, 1, 1);
 
        Vector2 dir = new Vector2(Screen.width, Screen.height);
        defaultAngle = Vector2.Angle(new Vector2(0, 1), dir);

        if(photonView.IsMine) 
        {
            localPlayer = this.gameObject;
            indicatorUIInstance.SetActive(false);
        }
    }
 
    private void OnEnable() {
        if(photonView.IsMine) localPlayer = this.gameObject;
    }

    void Update()
    {
        SetIndicator();
    }
 
 
    public void SetIndicator()
    {
        if(photonView.IsMine) return;
        if (!isOffScreen()) return;

        if(localPlayer == null)
        {
            Debug.Log("local player null");
            return; 
        }

        float angle = Vector2.Angle(new Vector2(0, 1), transform.position - localPlayer.transform.position);
        int sign = localPlayer.transform.position.x > transform.position.x ? -1 : 1;
        angle *= sign;
 
        Vector3 target = Camera.main.WorldToViewportPoint(transform.position);
 
        float x = target.x - 0.5f;
        float y = target.y - 0.5f;
 
        RectTransform indicatorRect = indicatorUIInstance.GetComponent<RectTransform>();
        
        if (-defaultAngle <= angle && angle <= defaultAngle)
        {
            Debug.Log("up");
            //anchor minY, maxY 0.96
 
            float anchorMinMaxY = 0.96f;
 
            float anchorMinMaxX = x * (anchorMinMaxY-0.5f) / y + 0.5f;
 
            if (anchorMinMaxX >= 0.94f) anchorMinMaxX = 0.94f;
            else if (anchorMinMaxX <= 0.06f) anchorMinMaxX = 0.06f;
 
            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
        else if (defaultAngle <= angle && angle <= 180 - defaultAngle)
        {
            Debug.Log("right");
            //anchor minX, maxX 0.94
 
            float anchorMinMaxX = 0.94f;
 
            float anchorMinMaxY = y * (anchorMinMaxX - 0.5f) / x + 0.5f;
 
            if (anchorMinMaxY >= 0.96f) anchorMinMaxY = 0.96f;
            else if (anchorMinMaxY <= 0.04f) anchorMinMaxY = 0.04f;
 
            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
        else if (-180 + defaultAngle <= angle && angle <= -defaultAngle)
        {
            Debug.Log("left");
            //anchor minX, maxX 0.06
 
            float anchorMinMaxX = 0.06f;
 
            float anchorMinMaxY = ( y * (anchorMinMaxX - 0.5f) / x ) + 0.5f;
 
            if (anchorMinMaxY >= 0.96f) anchorMinMaxY = 0.96f;
            else if (anchorMinMaxY <= 0.04f) anchorMinMaxY = 0.04f;
 
            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
        else if(-180 <= angle && angle <= -180 + defaultAngle || 180 - defaultAngle <=angle && angle <= 180)
        {
            Debug.Log("down");
            //anchor minY, maxY 0.04
 
            float anchorMinMaxY = 0.04f;
 
            float anchorMinMaxX = x * (anchorMinMaxY - 0.5f) / y + 0.5f;
 
            if (anchorMinMaxX >= 0.94f) anchorMinMaxX = 0.94f;
            else if (anchorMinMaxX <= 0.06f) anchorMinMaxX = 0.06f;
 
            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
 
        indicatorRect.anchoredPosition = new Vector3(0, 0, 0);
    }
 
 
    private bool isOffScreen()
    {
        Debug.Log("isOffScreen");
        Vector2 vec = Camera.main.WorldToViewportPoint(transform.position);
        // 화면에 보이는 경우
        if (vec.x >= 0 && vec.x <= 1 && vec.y >= 0 && vec.y <= 1) 
        {   
            indicatorUIInstance.SetActive(false);
            return false;
        }
        //아닌 경우
        else
        {
            indicatorUIInstance.SetActive(true);
            return true;
        }
    }

    private void OnDestroy()
    {
        Managers.Resource.Destroy(indicatorUIInstance);
    }
}

