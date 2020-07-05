using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class WaveModule : MonoBehaviour
{
    [Range(0f, 1f)] public float SelectLeft = 0f;
    [Range(0f, 1f)] public float SelectRight = 1f;
    public RawImage WaveImage;
    public Slider sliderLeft, sliderRight, sliderRate, sliderVolume;
    public AudioSource audioSource;
    public PulseBlockCenter getClipFrom;
    public Recorder recorder;

    Texture2D _texture;
    int _width, _heigth;
    Color32[] _texData;
    float _selectLeft = 0f, _selectRight = 1f, _sliderBeforeLeft, _sliderBeforeRight;
    AudioClip _ownClip, _lastInClip;
    void Start()
    {
        GenerateTexture();
        ApplySelectFromSliders();
    }

    void Update()
    {
        if (GetInClip() != _lastInClip)
        {
            _lastInClip = GetInClip();
            RefreshTexture();
        }
        if (_selectLeft != SelectLeft || _selectRight != SelectRight)
        {
            _selectLeft = SelectLeft;
            _selectRight = SelectRight;
            ApplySelect();
        }
    }

    AudioClip GetInClip()
    {
        if (_ownClip != null) return _ownClip;
        if (getClipFrom != null) return getClipFrom.Clip;
        if (recorder != null) return recorder.GetLastRecording();
        return null;
    }
    public void GenerateTexture(int width = 512, int height = 256)
    {
        var clip = GetInClip();
        if (clip == null) return;

        _width = width;
        _heigth = height;
        
        int texSizeX = width, texSizeY = height;
        _texture = new Texture2D(texSizeX, texSizeY, TextureFormat.RGBA32, false);
        _texture.filterMode = FilterMode.Point;

        var clipData = new float[clip.samples];
        clip.GetData(clipData, 0);
        var texData = new Color32[texSizeX * texSizeY];
        var pixelSamples = clip.samples / texSizeX;

        var halfSizeY = texSizeY / 2;
        for (var i = 0; i < texSizeX; i++)
        {
            var startInd = i * pixelSamples;
            float min = clipData[startInd], max = clipData[startInd];
            for (var j = startInd + 1; j < startInd + pixelSamples; j++)
            {
                min = Mathf.Min(min, clipData[j]);
                max = Mathf.Max(max, clipData[j]);
            }

            var maxY = (int)Mathf.Round(max * halfSizeY) + halfSizeY;
            maxY = Math.Min(maxY, texSizeY - 1);
            var minY = (int)Mathf.Round(min * halfSizeY) + halfSizeY;
            for (var j = minY - 2; j <= maxY + 2; j++)
            {
                texData[Index(i, j)] = new Color32(255, 255, 255, 255);
            }
        }
        
        _texData = texData;
        ApplyTexture();
    }

    public void ApplySelectFromSliders()
    {
        if (sliderLeft == null || sliderRight == null) return;

        if (_sliderBeforeLeft != sliderLeft.value)
        {
            SelectLeft = Mathf.Lerp(0f, _selectRight, sliderLeft.value);
            var otherVal = 1f - (_selectRight - SelectLeft) / (1 - SelectLeft);
            sliderRight.SetValueWithoutNotify(otherVal);
        }
        else if (_sliderBeforeRight != sliderRight.value)
        {
            SelectRight = Mathf.Lerp(_selectLeft, 1f, 1f - sliderRight.value);
            var otherVal = _selectLeft / SelectRight;
            sliderLeft.SetValueWithoutNotify(otherVal);
        }
        _sliderBeforeLeft = sliderLeft.value;
        _sliderBeforeRight = sliderRight.value;
    }
    public void ApplySelect()
    {
        var leftInd = (int) Mathf.Round(_selectLeft * _width);
        var rightInd = (int) Mathf.Round(_selectRight * _width);
        for (var x = 0; x < _width; x++)
        {
            var gray = x < leftInd || x > rightInd;
            for (var y = 0; y < _heigth; y++)
            {
                var c = _texData[Index(x, y)];
                if (c.r == 0 && c.g == 0 && c.b == 0 && c.a == 0) continue;
                c.a = gray ? (byte) 60 : (byte) 255;
                _texData[Index(x, y)] = c;
            }
        }
        ApplyTexture();
    }

    public void CutSelected()
    {
        _ownClip = GetOutClip();
        RefreshTexture();
    }

    public void CutAndSetGlobalClip()
    {
        PulseBlockCenter.Instance.Clip = GetOutClip();
    }

    void RefreshTexture()
    {
        sliderLeft.SetValueWithoutNotify(0);
        sliderRight.SetValueWithoutNotify(0);
        SelectLeft = 0f;
        SelectRight = 1f;
        SetDirty();
        GenerateTexture();
    }

    public void ClearOwn()
    {
        _ownClip = null;
    }

    const float MinRate = 1000, MaxRate = 87200;
    public void Play()
    {
        audioSource.clip = GetOutClip();
        UpdateVolume();
        audioSource.Play();
    }

    AudioClip _clipCache;

    public AudioClip GetOutClip()
    {
        if (_clipCache != null) return _clipCache;
        var mainClip = GetInClip();
        if (mainClip == null) return null;
        var sampleStart = (int) (mainClip.samples * _selectLeft);
        var sampleAmount = (int) (mainClip.samples * _selectRight) - sampleStart;
        var sampleRate = sliderRate == null ? 44100 : (int) Mathf.Lerp(MinRate, MaxRate, 1 - sliderRate.value);
        var newClip = ClipMaker.Make(mainClip, sampleStart, sampleAmount, sampleRate);
        _clipCache = newClip;
        return _clipCache;
    }
    public void SetDirty()
    {
        _clipCache = null;
    }

    public void UpdateVolume()
    {
        if (sliderVolume == null || audioSource == null) return;
        audioSource.volume = 1 - sliderVolume.value;
    }

    void ApplyTexture()
    {
        _texture.SetPixelData(_texData, 0);
        _texture.Apply();
        WaveImage.texture = _texture;
    }

    int Index(int x, int y)
    {
        x = Mathf.Clamp(x, 0, _width - 1);
        y = Mathf.Clamp(y, 0, _heigth - 1);
        return x + _width * y;
    }
}