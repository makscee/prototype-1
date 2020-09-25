using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class WavePartsContainer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public AudioClip clip;
    int _direction;
    [Range(1, 8)]public int resolutionDivisor = 2;
    public int[] splits = new int[0];
    float _selectFrom = 3000, _selectTo = 8000;
    public float thickness = 1;
    public RectTransform selector;
    bool _isDirty;
    [SerializeField] GameObject wavePartPrefab;

    List<WavePart> _waveParts = new List<WavePart>();

    RectTransform _rectTransform;
    RootBlock _rootBlock;

    public float SelectFrom
    {
        get => _selectFrom;
        set
        {
            _selectFrom = value;
            _isDirty = true;
        }
    }

    public float SelectTo
    {
        get => _selectTo;
        set
        {
            _selectTo = value;
            _isDirty = true;
        }
    }

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _direction = GetComponentInParent<DirectionIdHolder>().id;
        _rootBlock = Roots.Blocks[GetComponentInParent<RootIdHolder>().id];
        GameManager.OnNextFrame += () =>
        {
            _selectFrom = _rootBlock.soundsPlayer.Configs[_direction].SelectFrom;
            _selectTo = _rootBlock.soundsPlayer.Configs[_direction].SelectTo;
            Refresh();
        };
    }

    void Refresh()
    {
        _waveParts.Clear();
        _waveParts.AddRange(GetComponentsInChildren<WavePart>());
        while (_waveParts.Count < splits.Length + 1 && Application.isPlaying)
        {
            _waveParts.Add(Instantiate(wavePartPrefab, transform).GetComponent<WavePart>());
        }

        while (_waveParts.Count > splits.Length + 1 && Application.isPlaying)
        {
            Destroy(_waveParts[0]);
            _waveParts.RemoveAt(0);
        }
        if (_waveParts.Count != splits.Length + 1) return;
        
        var allSplits = new List<int> {0};
        allSplits.AddRange(splits);
        allSplits.Add(clip.samples);
        var totalHeight = _rectTransform.rect.height;
        var resolution = totalHeight / resolutionDivisor;
        for (var i = 0; i < allSplits.Count - 1; i++)
        {
            _waveParts[i].SamplesFrom = allSplits[i];
            _waveParts[i].SamplesTo = allSplits[i + 1];
            _waveParts[i].background.NumInPalette = 1 + i % 2;
            var length = 1f * _waveParts[i].SamplesTo - _waveParts[i].SamplesFrom;
            var heightPercent = length / clip.samples;
            _waveParts[i].layoutElement.preferredHeight = totalHeight * heightPercent;
            _waveParts[i].waveRenderer.resolution = (int) (resolution * heightPercent);
            _waveParts[i].waveRenderer.selectSamplesFrom = Mathf.RoundToInt(SelectFrom);
            _waveParts[i].waveRenderer.selectSamplesTo = Mathf.RoundToInt(SelectTo);
            _waveParts[i].waveRenderer.thickness = thickness;
        }

        var selectFromPos = totalHeight * SelectFrom / clip.samples;
        var selectHeight = totalHeight * (SelectTo - SelectFrom) / clip.samples;
        selector.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, selectFromPos, selectHeight);
        selector.SetAsLastSibling();
        
        _isDirty = false;
    }

    void UpdateSoundConfig()
    {
        _rootBlock.soundsPlayer.Configs[_direction].SelectFrom = Mathf.RoundToInt(SelectFrom);
        _rootBlock.soundsPlayer.Configs[_direction].SelectTo = Mathf.RoundToInt(SelectTo);
    }

    void OnValidate()
    {
        _isDirty = true;
    }

    void Update()
    {
        if (_isDirty)
        {
            Refresh();
        }
    }

    
    bool _dragTop, _dragging;
    public void OnDrag(PointerEventData eventData)
    {
        var sampleDelta = -Utils.ScaledScreenCoords(eventData.delta, transform).y / _rectTransform.rect.height * clip.samples;
        if (_dragTop) SelectFrom += sampleDelta;
        else SelectTo += sampleDelta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        
        var pos = Utils.ScaledScreenCoords(eventData.position, transform);
        var mid = _rectTransform.rect.height * (1f - (SelectFrom + (SelectTo - SelectFrom) / 2f) / clip.samples);
        _dragTop = pos.y > mid;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        UpdateSoundConfig();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging) return;

        var samplePos = ScreenPosToSamples(eventData.position);
        if (samplePos > _selectFrom && samplePos < _selectTo)
        {
            _rootBlock.soundsPlayer.Play(_direction);
            return;
        }

        var wavePart = _waveParts.Find(w => samplePos > w.SamplesFrom && samplePos < w.SamplesTo);
        SelectFrom = wavePart.SamplesFrom;
        SelectTo = wavePart.SamplesTo;
        UpdateSoundConfig();
    }

    int ScreenPosToSamples(Vector2 pos)
    {
        return Mathf.RoundToInt((1 - Utils.ScaledScreenCoords(pos, transform).y / _rectTransform.rect.height) * clip.samples);
    }
}