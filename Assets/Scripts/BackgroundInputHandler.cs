using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundInputHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    Camera _camera;

    public Action nextClickOverride;

    void Start()
    {
        _camera = SharedObjects.Instance.Camera;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount < 2 && !_draggingBlock)
            _camera.transform.position -= _camera.ScreenToWorldPoint(eventData.delta) - _camera.ScreenToWorldPoint(Vector3.zero);
        if (eventData.button == PointerEventData.InputButton.Left && Input.touchCount < 2)
            BlockEditor.OnBlockDrag();
    }

    bool _dragging, _draggingBlock;
    Block _draggedBlock;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || Input.touchCount > 1) return;
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
        if (eventData.button != PointerEventData.InputButton.Left || Input.touchCount > 1) return;
        _dragging = false;
        _draggingBlock = false;
        
        BlockEditor.OnBlockDragEnd();
        
        if (_draggedBlock != null)
            _draggedBlock.logic.EndDrag(eventData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging || Input.touchCount > 1 || eventData.button != PointerEventData.InputButton.Left) return;

        if (nextClickOverride != null)
        {
            nextClickOverride();
            nextClickOverride = null;
            return;
        }
        
        Utils.GetInputCoords(out var x, out var y);
        if (FieldMatrix.Get(x, y, out var block))
            block.logic.Click(eventData);
        else PixelDriver.Add(PixelRoad.Circle(Colors.GetPalette(-1)[3],
            3f, 3f, 0.05f, 0.5f, x, y).SetWeight(0.05f));
    }
}