using System;
using UnityEngine;

public class RootPanel : MonoBehaviour
{
    SlidingPanel _panel;
    DuoLine _line;
    int _id;

    void Start()
    {
        _panel = GetComponent<SlidingPanel>();
        _id = GetComponentInParent<RootIdHolder>().id;
        _panel.onOpen += CreateLine;
        _panel.onClose += ClearLine;
    }

    void CreateLine()
    {
        _line = DuoLine.Create(_panel.bookmark.icon, Roots.Root[_id].block.transform, false, true, _id);
    }

    void ClearLine()
    {
        Destroy(_line.gameObject);
    }
}