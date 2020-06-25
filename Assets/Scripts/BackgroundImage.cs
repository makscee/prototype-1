using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    Camera _camera;

    void Start()
    {
        _camera = SharedObjects.Instance.Camera;
    }

    bool needStartRefresh;
    public void OnDrag(PointerEventData eventData)
    {
        if (needStartRefresh)
        {
            needStartRefresh = false;
            RefreshStart(eventData.position);
        }
        if (Input.touchCount < 2)
            _camera.transform.position += (Vector3) (_start - (Vector2) _camera.ScreenToWorldPoint(eventData.position));
        else
            needStartRefresh = true;
    }

    Vector2 _start;
    bool _dragging;
    public void OnBeginDrag(PointerEventData eventData)
    {
        RefreshStart(eventData.pressPosition);
        _dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
    }

    void RefreshStart(Vector2 screenPos)
    {
        _start = _camera.ScreenToWorldPoint(screenPos);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging) return;
        foreach (var pulseBlock in PulseBlockCenter.Instance.PulseBlocks)
        {
            if (pulseBlock == null) break;
            pulseBlock.SoundsPlayer.ConfigRacksSetActive(false);
        }
    }
}