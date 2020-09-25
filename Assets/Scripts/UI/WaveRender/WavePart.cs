using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class WavePart : MonoBehaviour
{
    public LayoutElement layoutElement;
    WavePartsContainer _container;
    public WaveRenderer waveRenderer;
    int _samplesFrom, _samplesTo;
    public Painter background, wave;

    public int SamplesFrom
    {
        get => _samplesFrom;
        set
        {
            _samplesFrom = value;
            waveRenderer.samplesFrom = _samplesFrom;
            waveRenderer.SetVerticesDirty();
        }
    }

    public int SamplesTo
    {
        get => _samplesTo;
        set
        {
            _samplesTo = value;
            waveRenderer.samplesTo = _samplesTo;
            waveRenderer.SetVerticesDirty();
        }
    }

    void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        _container = GetComponentInParent<WavePartsContainer>();
        waveRenderer = GetComponentInChildren<WaveRenderer>();
        waveRenderer.clip = _container.clip;
    }
}