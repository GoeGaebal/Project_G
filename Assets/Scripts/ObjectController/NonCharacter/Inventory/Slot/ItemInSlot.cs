using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemInSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
            if (Managers.Network.LocalPlayer != null)
            {
                // TODO : PUN2에서 교체해야함
                //사과 개수만큼 드랍
                Managers.Network.Client.Send(
                    new C_SpawnLooting()
                    {
                        ObjectId = item.ID,
                        Count = count,
                        PlayerId = Managers.Network.LocalPlayer.Id
                    }
                );
                RemoveItem();//인벤토리에서 삭제
            }
            
        }
        _icon.raycastTarget = true;
        transform.SetParent(parentAfterDrag);//해당 위치의 슬롯에 아이템 저장}
    }
}
