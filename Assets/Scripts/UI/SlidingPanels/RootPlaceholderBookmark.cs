using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RootPlaceholderBookmark : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] RootBlockPlacer rootBlockPlacer;
    int _position;
    void Start()
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
        var offset = SlidingPanelBookmark.GetHeightPositionByIndex(ind, transform);
        transform.position = new Vector3(transform.position.x, offset);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        rootBlockPlacer.StartPlacing();
    }
}