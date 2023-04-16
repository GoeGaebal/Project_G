using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Transform _target;//UI 위치
    private Vector2 _begin;//시작 좌표
    private Vector2 _move;//마우스를 클릭했을 때 마우스의 좌표

    private void Awake()
    {
        if (_target == null)
        {
            _target = transform.parent;
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _begin = _target.position;//시작 좌표
        _move = eventData.position;//마우스를 클릭했을 때 마우스의 좌표
    }

    void IDragHandler.OnDrag(PointerEventData eventData)//드래그 중
    {
        _target.position = _begin + (eventData.position - _move);
        //최종 좌표-마우스의 좌표 = 이동 벡터.
        //이동 벡터를 초기 위치에 더해주면 UI를 이동시킬 수 있다.
    }
}
