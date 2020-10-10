using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigPanel : MonoBehaviour
{
    int _direction, _id;
    public SplitSlider rate, volume;
    SlidingPanel _panel;
    List<DuoLine> _lines = new List<DuoLine>();

    void Start()
    {
        _panel = GetComponent<SlidingPanel>();
        _direction = GetComponentInParent<DirectionIdHolder>().id;
        _panel.onOpen += DrawLines;
        _panel.onClose += ClearLines;
        GameManager.OnNextFrame += () =>
        {
            _id = GetComponentInParent<RootIdHolder>().id;
            var rootBlock = Roots.Root[_id].block;
            var soundsPlayer = rootBlock.soundsPlayer;
            var config = soundsPlayer.Configs[_direction]; 
            rate.InitValue(config.Rate);
            volume.InitValue(config.Volume * 100);
            rate.onValueChange += v =>
                config.Rate = Mathf.RoundToInt(v);
            volume.onValueChange += v =>
                config.Volume = v / 100;

            rate.onClick += () => soundsPlayer.Play(_direction);
            volume.onClick += () => soundsPlayer.Play(_direction);
        };
    }

    void DrawLines()
    {
        foreach (var node in Roots.Root[_id].DeadEnds)
        {
            if (node.dirs[_direction])
            {
                var duoLine = DuoLine.Create(_panel.bookmark.icon, node.transform, false, true, _id);
                _lines.Add(duoLine);
            }
        }
    }

    void ClearLines()
    {
        foreach (var line in _lines)
        {
            Destroy(line.gameObject);
        }
        _lines.Clear();
    }
}