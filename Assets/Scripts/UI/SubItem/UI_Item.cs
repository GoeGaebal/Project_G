using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Item : UI_Base
{
    enum Texts
    {
        Count
    }
    
    [SerializeField] private Image _icon; //아이템의 이미지
    GameObject[] players;
    GameObject player;
    public Text countText;//개수 텍스트

    [HideInInspector] public Item item;//아이템
    [HideInInspector] public int count = 1;//아이템 개수
    [HideInInspector] public Transform parentAfterDrag;

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        
        _icon = GetComponent<Image>();
        countText = GetText((int)Texts.Count);
        
        AddUIEvent(gameObject,OnBeginDrag,Define.UIEvent.BeginDrag);
        AddUIEvent(gameObject,OnDrag, Define.UIEvent.Drag);
        AddUIEvent(gameObject,OnDrop, Define.UIEvent.Drop);
        AddUIEvent(gameObject,OnEndDrag, Define.UIEvent.EndDrag);
        
        players = GameObject.FindGameObjectsWithTag("Player");//씬에 있는 플레이어들 중
        foreach (GameObject p in players)
        {
            PhotonView photonView = p.GetPhotonView();
            if (photonView != null && photonView.IsMine)//내 플레이어 오브젝트 찾기
            {
                player = p;
            }
        }
    }

    public void InitializeItem(Item newItem)//슬롯의 아이콘을 해당 아이템의 것으로 변경
    {
        Init();
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
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public void OnDrag(PointerEventData eventData)//드래그 중
    {
        transform.position = eventData.position;
    }
    
    // TODO: Slot과 Item이 거의 비슷한 동작을 하는데 이는 EventHandler가 둘다 OnDrop을 받기 때문에 어느 것이 반응하더라도 동작하게끔 설계되어 있음
    // 중복된 코드이므로 정리가 필요함
    public void OnDrop(PointerEventData eventData)// 슬롯 안에 아이템이 존재하여 먼저 반응할 때
    {
        UI_Item item = eventData.pointerDrag.GetComponent<UI_Item>();
        if (transform.parent.GetComponent<UI_Slot>() != null) //슬롯을 부모로 가질 시에 서로 바꾼다.
        {
            var parentTransform = transform.parent.transform;
            parentAfterDrag = item.parentAfterDrag;
            transform.SetParent(item.parentAfterDrag);
            
            item.parentAfterDrag = parentTransform;
            item.transform.SetParent(parentTransform);
        }
    }

    public void OnEndDrag(PointerEventData eventData) // 마우스를 뗄 때
    {
        if (!EventSystem.current.IsPointerOverGameObject())//UI 바깥으로 드래그하면 필드에 아이템 드랍하고 인벤토리에서 제거
        {
            if (player != null)
            {
                //사과 개수만큼 드랍
                Managers.Object.SpawnLootings(item.ID, count, player.gameObject.transform.position, 1.5f, 1.0f);
                RemoveItem();//인벤토리에서 삭제
            }
            
        }
        _icon.raycastTarget = true;
        transform.SetParent(parentAfterDrag);//해당 위치의 슬롯에 아이템 저장}
    }
}
