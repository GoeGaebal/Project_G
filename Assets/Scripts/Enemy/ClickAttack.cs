using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAttack : MonoBehaviour
{
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        //임시 코드
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the mouse position
            

            // Check if the ray hits a game object
             Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
 
            // Raycast함수를 통해 부딪치는 collider를 hit에 리턴받습니다.
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

            if(hit.collider != null)
            {
                // Select the game object
                IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                if(damageable != null)  damageable.OnDamage(10.0f);
            }
        }
    }
}
