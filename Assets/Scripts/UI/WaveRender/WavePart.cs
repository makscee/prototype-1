using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class WavePart : MonoBehaviour
{
    public LayoutElement layoutElement;
    public WaveRenderer waveRenderer;
    int _samplesFrom, _samplesTo;
    public Painter background, wave;
    public RectTransform rectTransform;

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
        rectTransform = GetComponent<RectTransform>();
        layoutElement = GetComponent<LayoutElement>();
        waveRenderer = GetComponentInChildren<WaveRenderer>();
    }
}