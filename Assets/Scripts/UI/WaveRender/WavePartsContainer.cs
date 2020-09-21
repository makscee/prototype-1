using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WavePartsContainer : MonoBehaviour
{
    public AudioClip clip;
    public int[] splits = new int[0];
    int[] _appliedSplits;
    [SerializeField] GameObject wavePartPrefab;

    List<WavePart> _waveParts = new List<WavePart>();

    RectTransform _rectTransform;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    void Refresh()
    {
        _waveParts.Clear();
        _waveParts.AddRange(GetComponentsInChildren<WavePart>());
        while (_waveParts.Count < splits.Length + 1)
        {
            _waveParts.Add(Instantiate(wavePartPrefab, transform).GetComponent<WavePart>());
        }

        while (_waveParts.Count > splits.Length + 1)
        {
            Destroy(_waveParts[0]);
            _waveParts.RemoveAt(0);
        }
        
        var allSplits = new List<int> {0};
        allSplits.AddRange(splits);
        allSplits.Add(clip.samples);
        var totalHeight = _rectTransform.rect.height;
        for (var i = 0; i < allSplits.Count - 1; i++)
        {
            _waveParts[i].SamplesFrom = allSplits[i];
            _waveParts[i].SamplesTo = allSplits[i + 1];
            _waveParts[i].background.NumInPalette = 1 + i % 2;
            var length = 1f * _waveParts[i].SamplesTo - _waveParts[i].SamplesFrom;
            _waveParts[i].layoutElement.preferredHeight = totalHeight * length / clip.samples;
        }

        _appliedSplits = splits.ToArray();
    }

    void Update()
    {
        if (!splits.SequenceEqual(_appliedSplits))
        {
            Refresh();
        }
    }
}