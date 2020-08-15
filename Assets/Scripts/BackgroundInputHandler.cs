using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundInputHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    Camera _camera;

    void Start()
    {
        _camera = SharedObjects.Instance.Camera;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount < 2 && !_draggingBlock)
            _camera.transform.position -= _camera.ScreenToWorldPoint(eventData.delta) - _camera.ScreenToWorldPoint(Vector3.zero);
        
        BlockEditor.OnBlockDrag();
    }

    bool _dragging, _draggingBlock;
    Block _draggedBlock;
    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        
        Utils.GetInputCoords(out var x, out var y);
        if (FieldMatrix.Get(x, y, out _draggedBlock))
        {
            _draggingBlock = true;
            _draggedBlock.logic.BeginDrag(eventData);
            BlockEditor.OnBlockDragStart(_draggedBlock);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        _draggingBlock = false;
        
        BlockEditor.OnBlockDragEnd();
        
        if (_draggedBlock != null)
            _draggedBlock.logic.EndDrag(eventData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging || Input.touchCount > 1) return;
        
        Utils.GetInputCoords(out var x, out var y);
        if (FieldMatrix.Get(x, y, out var block))
            block.logic.Click(eventData);
    }
}