using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemInSlot : MonoBehaviourPun, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _icon;//아이템의 이미지
    GameObject[] players;
    GameObject player;
    public Text countText;//개수 텍스트

    [HideInInspector] public Item item;//아이템
    [HideInInspector] public int count = 1;//아이템 개수
    [HideInInspector] public Transform parentAfterDrag;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");//씬에 있는 플레이어들 중
        foreach (GameObject p in players)
        {
            PhotonView photonView = p.GetPhotonView();
            if (photonView != null && photonView.IsMine)//내 플레이어 오브젝트 찾기
            {
                player = p;
            }
        }
        InitializeItem(item);
    }

    public void InitializeItem(Item newItem)//슬롯의 아이콘을 해당 아이템의 것으로 변경
    {
        item = newItem;
        _icon.sprite = item.Icon;
        RefreshCount();
    }
    public void RemoveItem()//슬롯의 아이템 삭제
    {
        Destroy(gameObject);
    }

    public void RefreshCount()//아이템 개수 표시
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
        
    }
    public void OnBeginDrag(PointerEventData eventData)//클릭했을 때
    {
        _icon.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)//드래그 중
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)//드랍했을 때
    {
        if (!EventSystem.current.IsPointerOverGameObject())//UI 바깥으로 드래그하면 필드에 아이템 드랍하고 인벤토리에서 제거
        {
            if (player != null)
            {
                //사과 개수만큼 드랍
                Managers.Object.SpawnLootingItems(item.ID, count, player.gameObject.transform.position, 1.5f, 1.0f);
                RemoveItem();//인벤토리에서 삭제
            }
            
        }
        _icon.raycastTarget = true;
        transform.SetParent(parentAfterDrag);//해당 위치의 슬롯에 아이템 저장}
    }
}
