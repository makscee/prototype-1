using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlidingPanelBookmark : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Action<Vector2> onDrag;
    public Action onBeginDrag;
    public Action onClick;
    public Transform icon;

    public const float OffsetByScreenDivision = 6, OffsetByPixels = 100;

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

    public static float GetHeightPositionByIndex(int index, Transform transform)
    {
        var offsetPos = Screen.height / OffsetByScreenDivision;
        var scaledOffsetValue = Utils.ScaledScreenCoords(new Vector2(0, OffsetByPixels), transform, true);
        var offset = offsetPos + scaledOffsetValue.y * index;
        return offset;
    }
}