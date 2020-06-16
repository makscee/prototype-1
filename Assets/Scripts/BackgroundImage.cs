using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
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
    public void OnBeginDrag(PointerEventData eventData)
    {
        RefreshStart(eventData.pressPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    void RefreshStart(Vector2 screenPos)
    {
        _start = _camera.ScreenToWorldPoint(screenPos);
    }
}