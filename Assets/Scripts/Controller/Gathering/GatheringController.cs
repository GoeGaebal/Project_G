using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GatheringController : DamageableEntity
{
    private Vector3 endPosition = new Vector3(2,0,0);

    private float currentTIme = 0;
    private float lerpTime = 2.0f;

    private void Start()
    {
        transform.position = Vector3.zero;
    }

    private void Update()
    {
        
    }

    public void Bouncing(Vector3 endPosition,float duration)
    {
        StartCoroutine(BouncingPosition(endPosition, duration));
    }
    
    IEnumerator BouncingPosition(Vector3 targetPosition,float duration, float maxHeight = 1.0f,int bounceCount = 5,float cof = 0.6f, float threshold = 1.0f)
    {
        // 초기 값 세팅
        float time = 0.0f;
        float curH = 0.0f;
        float Sn = (1.0f - Mathf.Pow(cof, bounceCount)) / (1.0f - cof);
        
        float hSpeed = (4.0f*maxHeight*Sn)/ duration;
        float gravity = (2.0f * hSpeed * Sn) / duration;
        Vector3 startPosition = transform.position;
        
        while (time < duration)
        {
            // apply gravity if the entity is in the air:
            if (curH > 0) hSpeed -= gravity * Time.deltaTime;
            // add speed to altitude:
            curH += hSpeed * Time.deltaTime;
            // if the entity did hit the ground:
            if (curH < 0) {
                // simulate a bounce (to avoid negative Z):
                curH = -curH;
                // if the entity was falling down, reduce the speed: 
                if (hSpeed < 0.0f) hSpeed = -hSpeed * cof;
                // if the speed is now below the threshold, snap the entity to the ground:
                if (hSpeed < threshold) {
                    curH = 0.0f;
                    hSpeed = 0.0f;
                }
            }
            
            float t = time / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.up * curH + Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
