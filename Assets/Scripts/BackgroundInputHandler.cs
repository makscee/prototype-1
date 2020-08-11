using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundInputHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    Camera _camera;
    BlockOld _draggedBlock;

    void Start()
    {
        _camera = SharedObjects.Instance.Camera;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount < 2)
        {
            if (_draggedBlock != null)
                _draggedBlock = BlockEditor.OnBlockDrag();
            else _camera.transform.position -= _camera.ScreenToWorldPoint(eventData.delta) - _camera.ScreenToWorldPoint(Vector3.zero);
        }
    }

    bool _dragging;
    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        // TryGetDraggedMaskedBlock();
        // if (_draggedBlock != null) BlockEditor.OnBlockDragStart(_draggedBlock);
    }

    void TryGetDraggedMaskedBlock()
    {
        Utils.GetInputCoords(out var x, out var y);
        // if (!FieldMatrix.Get(x, y, out _draggedBlock)) return;
        if (!_draggedBlock.masked) _draggedBlock = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
    }

    Block _last;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging || Input.touchCount > 1) return;
        Utils.GetInputCoords(out var x, out var y);
        if (_last == null)
            _last = NodeBlock.Create(x, y);
        else _last = NodeBlock.Create(x, y, _last);
        _last.transform.position = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);
        // if (FieldMatrix.Get(x, y, out var block))
        //     BlockEditor.OnBlockClick(block);
    }
}