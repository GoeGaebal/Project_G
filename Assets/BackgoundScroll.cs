using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgoundScroll : MonoBehaviour
{

    [SerializeField]private float speed = 0.5f;
    public Transform[] backgrounds;
 
    [SerializeField]private  float leftPosX = 0f;
    [SerializeField]private float rightPosX = 0f;

  


    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].position += new Vector3(-speed, 0, 0) * Time.deltaTime;
 
            if(backgrounds[i].position.x < leftPosX)
            {
                Vector3 nextPos = backgrounds[i].position;
                nextPos = new Vector3(rightPosX, nextPos.y, nextPos.z);
                backgrounds[i].position = nextPos;
            }
        }
    }
    
}
