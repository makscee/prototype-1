using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RootPlaceholderBookmark : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] RootBlockPlacer rootBlockPlacer;
    int _position;
    void Awake()
    {
        Reposition();
    }

    void Update()
    {
        if (_position != Roots.Count) Reposition();
    }

    void Reposition()
    {
        transform.SetAsFirstSibling();
        var ind = Roots.Count;
        _position = ind;
        var offsetPos = Screen.height / SlidingPanelBookmark.OffsetByScreenDivision;
        var indOffsetPos = Utils.ScaledScreenCoords(new Vector2(0, ind * SlidingPanelBookmark.OffsetByPixels), transform, true).y;
        var offset = offsetPos + indOffsetPos;
        transform.position = new Vector3(transform.position.x, offset);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        rootBlockPlacer.StartPlacing();
    }
}