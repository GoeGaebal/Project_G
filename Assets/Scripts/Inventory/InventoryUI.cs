using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Range(1, 20)]
        [SerializeField] private int _slotRow = 2;//슬롯 행 개수
        [Range(1, 20)]
        [SerializeField] private int _slotColumn = 2;//슬롯 열 개수
        [SerializeField] private float _slotMarginX = 10f;//슬롯 내부 여백
        [SerializeField] private float _slotMarginY = 5f;//슬롯 내부 여백
        [SerializeField] private float _slotPadding = 10f;//인벤토리 전체 여백
        [Range(16, 64)]
        [SerializeField] private float _slotSize = 32f;//슬롯 한 개 크기
        [SerializeField] private RectTransform _slotArea;//슬롯 한 개 영역
        [SerializeField] private GameObject _ItemSlot;//슬롯 프리팹
        [SerializeField] private List<ItemSlotUI> _slotList = new List<ItemSlotUI>();
        [SerializeField] private Vector2 _anchorMin = new Vector2(0, 1);
        [SerializeField] private Vector2 _anchorMax = new Vector2(0, 1);

        private void Awake()
        {
            CreateItemSlots();
        }

        private void CreateItemSlots()
        {
            _ItemSlot.TryGetComponent(out RectTransform rect);
            rect.sizeDelta = new Vector2(_slotSize, _slotSize);

            _ItemSlot.TryGetComponent(out ItemSlotUI ui);
            if (ui == null)
            {
                _ItemSlot.AddComponent<ItemSlotUI>();
            }
            _ItemSlot.SetActive(false);

            Vector2 beginPosition = new Vector2(_slotPadding, -_slotPadding);
            Vector2 currentPosition = beginPosition;

            _slotList = new List<ItemSlotUI>(_slotColumn * _slotRow);

            for (int i = 0; i < _slotColumn; i++)
            {
                for (int j = 0; j < _slotRow; j++)
                {
                    int index = (_slotRow * i) + j;

                    var slotRect = CloneSlot();
                    slotRect.pivot = new Vector2(0f, 1f);
                    slotRect.anchorMin = _anchorMin;
                    slotRect.anchorMax = _anchorMax;
                    slotRect.anchoredPosition = currentPosition;
                    slotRect.gameObject.SetActive(true);
                    slotRect.gameObject.name = $"Item Slot [{index}]";

                    var slotUI = slotRect.GetComponent<ItemSlotUI>();
                    slotUI.SetSlotIndex(index);
                    _slotList.Add(slotUI);

                    currentPosition.x += (_slotMarginX + _slotSize);
                }
                currentPosition.x = beginPosition.x;
                currentPosition.y -= (_slotMarginY + _slotSize);
            }

            if (_ItemSlot.scene.rootCount != 0)
            {
                Destroy(_ItemSlot);
            }

            RectTransform CloneSlot()
            {
                GameObject slot = Instantiate(_ItemSlot);
                RectTransform rt = slot.GetComponent<RectTransform>();
                rt.SetParent(_slotArea);

                return rt;
            }
        }
    }
}
