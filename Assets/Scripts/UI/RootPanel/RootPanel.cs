using System;
using UnityEngine;

public class RootPanel : MonoBehaviour
{
    [SerializeField] SplitSlider bpmSlider;
    SlidingPanel _panel;
    [SerializeField] DuoLine _line;
    int _id;

    void Start()
    {
        _panel = GetComponent<SlidingPanel>();
        _id = GetComponentInParent<RootIdHolder>().id;
        _panel.onOpen += CreateLine;
        _panel.onClose += ClearLine;
        bpmSlider.onValueChange = v => Roots.Root[_id].pulse.Bpm = Mathf.Round(v);
    }

    void CreateLine()
    {
        _line = DuoLine.Create(_panel.bookmark.icon, Roots.Root[_id].block.transform, false, true, _id);
    }

    void ClearLine()
    {
        if (_line != null)
            Destroy(_line.gameObject);
    }
}