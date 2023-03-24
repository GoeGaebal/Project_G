using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Transform _target;//UI 시작 위치
    private Vector2 _begin;
    private Vector2 _move;

    private void Awake()
    {
        if (_target == null)
        {
            _target = transform.parent;
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _begin = _target.position;
        _move = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _target.position = _begin + (eventData.position - _move);
    }
}
