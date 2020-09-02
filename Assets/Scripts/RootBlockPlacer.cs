using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RootBlockPlacer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] RawImage background;
    [SerializeField] Palette palette;
    [SerializeField] Transform blockVisual;
    Camera _camera;

    void Start()
    {
        _camera = SharedObjects.Instance.Camera;
    }

    void Update()
    {
        blockVisual.position = (Vector2)_camera.transform.position;
    }

    const float DarkenAlpha = 0.6f;
    public void StartPlacing()
    {
        gameObject.SetActive(true);
        SharedObjects.Instance.configCanvas.Disable();
        palette.ColorsId = Colors.GetRandomFreeId();
        background.color = new Color(0, 0, 0, DarkenAlpha);
    }

    void FinishPlacing()
    {
        Animator.Interpolate(DarkenAlpha, 0f, 1f).Type(InterpolationType.Linear)
            .PassValue(v => background.color = new Color(0, 0, 0, v))
            .WhenDone(() =>
            {
                gameObject.SetActive(false);
                SharedObjects.Instance.configCanvas.Enable();
                var position = _camera.transform.position;
                int x = Mathf.RoundToInt(position.x), y = Mathf.RoundToInt(position.y);
                var newRootBlock = RootBlock.Create(x, y);
                Roots.Palettes(newRootBlock.rootNum).ColorsId = palette.ColorsId;
            });
    }

    bool _dragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _camera.transform.position -= _camera.ScreenToWorldPoint(eventData.delta) - _camera.ScreenToWorldPoint(Vector3.zero);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        SnapToGrid();
    }

    void SnapToGrid()
    {
        var position = _camera.transform.position;
        var snappedPos = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
        Animator.Interpolate(position, snappedPos, 0.2f).Type(InterpolationType.InvSquare)
            .PassDelta(v => _camera.transform.position += v);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging) return;
        FinishPlacing();
    }
}