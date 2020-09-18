using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlidingPanelBookmark : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Action<Vector2> onDrag;
    public Action onBeginDrag;
    public Action onClick;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging) return;
        onClick?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData.delta);
    }

    bool _dragging;
    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        onBeginDrag?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
    }
}