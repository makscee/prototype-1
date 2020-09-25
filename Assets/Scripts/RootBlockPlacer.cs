using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RootBlockPlacer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] RawImage background;
    [SerializeField] Palette palette;
    [SerializeField] Transform blockVisual;
    [SerializeField] Camera cam;

    void Update()
    {
        blockVisual.position = (Vector2)cam.transform.position;
    }

    const float DarkenAlpha = 0.6f;
    public void StartPlacing()
    {
        gameObject.SetActive(true);
        palette.ColorsId = Colors.GetRandomFreeId();
        background.color = new Color(0, 0, 0, DarkenAlpha);
        SnapToGrid();
    }

    void FinishPlacing()
    {
        Animator.Interpolate(DarkenAlpha, 0f, 1f).Type(InterpolationType.Linear)
            .PassValue(v => background.color = new Color(0, 0, 0, v))
            .WhenDone(() =>
            {
                gameObject.SetActive(false);
                var position = cam.transform.position;
                int x = Mathf.RoundToInt(position.x), y = Mathf.RoundToInt(position.y);
                Roots.CreateRoot(x, y, -1, palette.ColorsId);
            });
    }

    bool _dragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        cam.transform.position -= cam.ScreenToWorldPoint(eventData.delta) - cam.ScreenToWorldPoint(Vector3.zero);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        SnapToGrid();
    }

    void SnapToGrid()
    {
        var position = cam.transform.position;
        var snappedPos = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
        Animator.Interpolate(position, snappedPos, 0.2f).Type(InterpolationType.InvSquare)
            .PassDelta(v => cam.transform.position += v);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging) return;
        FinishPlacing();
    }
}