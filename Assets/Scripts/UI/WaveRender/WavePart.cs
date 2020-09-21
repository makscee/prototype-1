using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class WavePart : MonoBehaviour
{
    public LayoutElement layoutElement;
    WavePartsContainer _container;
    WaveRenderer _waveRenderer;
    int _samplesFrom, _samplesTo;
    public Painter background, wave;

    public int SamplesFrom
    {
        get => _samplesFrom;
        set
        {
            _samplesFrom = value;
            _waveRenderer.samplesFrom = _samplesFrom;
            _waveRenderer.SetVerticesDirty();
        }
    }

    public int SamplesTo
    {
        get => _samplesTo;
        set
        {
            _samplesTo = value;
            _waveRenderer.samplesTo = _samplesTo;
            _waveRenderer.SetVerticesDirty();
        }
    }

    void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        _container = GetComponentInParent<WavePartsContainer>();
        _waveRenderer = GetComponentInChildren<WaveRenderer>();
        _waveRenderer.clip = _container.clip;
    }
}