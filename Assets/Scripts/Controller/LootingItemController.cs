using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class LootingItemController : MonoBehaviourPun
{
    public int id = 1;
    public int guid = 1;
    [Header("Physics")]
    [Tooltip("충돌계수")]
    [Range(0.0f,1.0f)]
    [SerializeField] private float cof;
    [Tooltip("튕겨지는 개수")]
    [SerializeField] private int bounceCount;
    [Tooltip("임계 속도")]
    [SerializeField] private float threshold;
    [SerializeField] private Item item;
    public Item GetItem => item;
    private UI_Inven ui_inven;

    private float Sn;

    private void Start()
    {
        ui_inven = GameObject.FindObjectOfType<UI_Inven>();
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        Invoke("EnableCollider", 0.7f);
    }

    private void Init()
    {
        cof = Managers.Data.LootingDict[id].cof;
        bounceCount = Managers.Data.LootingDict[id].bounceCount;
        threshold = Managers.Data.LootingDict[id].threshold;
        Sn = Managers.Data.LootingDict[id].Sn;
    }

    public void Bounce(Vector3 endPosition,float duration, float maxHeight = 1.0f)
    {
        Init();
        StartCoroutine(CoCalcBouncePos(endPosition, duration, maxHeight));
    }

    private IEnumerator CoCalcBouncePos(Vector3 targetPosition,float duration, float maxHeight)
    {
        // 초기 값 세팅
        float time = 0.0f;
        float curH = 0.0f;
        
        // TODO : 등비수열의 합을 코루틴 시작마다 계산 중임 추후에 데이터베이스 계산된 값을 넣고 불러오도록 하자
        // Sn = (1.0f - Mathf.Pow(cof, bounceCount)) / (1.0f - cof);
        float hSpeed = (4.0f * maxHeight * Sn) / duration;
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

    public void EnableCollider()
    {
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (collision.gameObject.GetPhotonView().IsMine)
                {
                    if (ui_inven.AddItem(item))
                    {
                        Debug.Log("아이템 획득 성공");
                        Managers.Network.BroadCastObjectDestroy(guid);
                    }
                    else
                    {
                        Debug.Log("아이템 획득 실패");
                    }
                }
                else
                {
                    Managers.Network.SendLootingAddItem(collision.gameObject.GetPhotonView().ViewID,guid);
                    Managers.Network.BroadCastObjectDestroy(guid);
                }
            }
        }
    }
    
    public void ApplyDie()
    {
        Managers.Object.LocalObjectsDict.Remove(guid);
        Managers.Object.ObjectInfos.Remove(guid);
        Managers.Resource.Destroy(gameObject);
    }
}
