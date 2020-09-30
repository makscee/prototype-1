using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class WavePartsContainer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public SlicedAudioClip slicedAudioClip => Roots.Root[_rootBlock.rootId].slicedClip;
    public UpDownButton recordButton;
    int _direction;
    [Range(1, 8)]public int resolutionDivisor = 2;
    [SerializeField] float _selectFrom = 3000, _selectTo = 8000;
    public float thickness = 1;
    public RectTransform selector;
    bool _isDirty;
    [SerializeField] GameObject wavePartPrefab;
    [SerializeField] Transform wavePartsParent;

    List<WavePart> _waveParts = new List<WavePart>();

    RectTransform _rectTransform;
    RootBlock _rootBlock;

    public float SelectFrom
    {
        get => _selectFrom;
        set
        {
            _selectFrom = Mathf.Clamp(value, slicedAudioClip.slices[0], slicedAudioClip.slices.Last());
            _isDirty = true;
        }
    }

    public float SelectTo
    {
        get => _selectTo;
        set
        {
            _selectTo = Mathf.Clamp(value, slicedAudioClip.slices[0], slicedAudioClip.slices.Last());
            _isDirty = true;
        }
    }

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _direction = GetComponentInParent<DirectionIdHolder>().id;
        recordButton.OnDown = StartRecording;
        recordButton.OnUp = EndRecording;
        GameManager.OnNextFrame += () =>
        {
            _rootBlock = Roots.Root[GetComponentInParent<RootIdHolder>().id].block;
            _selectFrom = _rootBlock.soundsPlayer.Configs[_direction].SelectFrom;
            _selectTo = _rootBlock.soundsPlayer.Configs[_direction].SelectTo;
            Refresh();
        };
    }

    void Refresh()
    {
        _waveParts.Clear();
        _waveParts.AddRange(wavePartsParent.GetComponentsInChildren<WavePart>());
        while (_waveParts.Count < slicedAudioClip.slices.Count - 1 && Application.isPlaying)
        {
            _waveParts.Add(Instantiate(wavePartPrefab, wavePartsParent).GetComponent<WavePart>());
        }

        while (_waveParts.Count > slicedAudioClip.slices.Count - 1 && Application.isPlaying)
        {
            Destroy(_waveParts[0].gameObject);
            _waveParts.RemoveAt(0);
        }
        if (_waveParts.Count != slicedAudioClip.slices.Count - 1) return;
        
        var totalHeight = _rectTransform.rect.height;
        var resolution = totalHeight / resolutionDivisor;
        for (var i = 0; i < slicedAudioClip.slices.Count - 1; i++)
        {
            _waveParts[i].SamplesFrom = slicedAudioClip.slices[i];
            _waveParts[i].SamplesTo = slicedAudioClip.slices[i + 1];
            _waveParts[i].background.NumInPalette = 1 + i % 2;
            var length = 1f * _waveParts[i].SamplesTo - _waveParts[i].SamplesFrom;
            var heightPercent = length / slicedAudioClip.Samples;
            _waveParts[i].layoutElement.preferredHeight = totalHeight * heightPercent;
            _waveParts[i].waveRenderer.resolution =  Mathf.RoundToInt(resolution * heightPercent);
            _waveParts[i].waveRenderer.selectSamplesFrom = Mathf.RoundToInt(SelectFrom);
            _waveParts[i].waveRenderer.selectSamplesTo = Mathf.RoundToInt(SelectTo);
        }

        var selectFromPos = totalHeight * (SelectFrom - slicedAudioClip.slices[0]) / slicedAudioClip.Samples;
        var selectHeight = totalHeight * (SelectTo - SelectFrom) / slicedAudioClip.Samples;
        selector.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, selectFromPos, selectHeight);
        
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
        if (_recording)
        {
            _isDirty = true;
            slicedAudioClip.UpdateRecording();
        }
        if (_isDirty)
        {
            Refresh();
        }
    }

    
    bool _dragTop, _dragging;
    public void OnDrag(PointerEventData eventData)
    {
        var sampleDelta = -Utils.ScaledScreenCoords(eventData.delta, transform).y / _rectTransform.rect.height * slicedAudioClip.Samples;
        if (_dragTop) SelectFrom += sampleDelta;
        else SelectTo += sampleDelta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        
        var pos = Utils.ScaledScreenCoords(eventData.position, transform);
        var mid = _rectTransform.rect.height * (1f - (SelectFrom - slicedAudioClip.slices[0] + (SelectTo - SelectFrom) / 2f) / slicedAudioClip.Samples);
        _dragTop = pos.y > mid;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        UpdateSoundConfig();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging || _recording) return;

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

    bool _recording;
    public void StartRecording()
    {
        slicedAudioClip.StartRecording();
        _recording = true;
    }

    public void EndRecording()
    {
        slicedAudioClip.EndRecording();
        Refresh();
        _recording = false;
    }

    int ScreenPosToSamples(Vector2 pos)
    {
        var result = Mathf.RoundToInt((1 - pos.y / Screen.height) * slicedAudioClip.Samples + slicedAudioClip.slices[0]);
        // var result = Mathf.RoundToInt((1 - Utils.ScaledScreenCoords(pos, transform).y / _rectTransform.rect.height) * slicedAudioClip.Samples + slicedAudioClip.slices[0]);
        return result;
    }
}