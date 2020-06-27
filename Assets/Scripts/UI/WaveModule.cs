using System;
using UnityEngine;
using UnityEngine.UI;

public class WaveModule : MonoBehaviour
{
    [Range(0f, 1f)] public float SelectLeft = 0f;
    [Range(0f, 1f)] public float SelectRight = 1f;
    public RawImage WaveImage;
    public Slider SliderLeft, SliderRight, SliderRate, SliderVolume;
    public AudioSource AudioSource;
    
    Texture2D _texture;
    int _width, _heigth;
    Color32[] _texData;
    float _selectLeft = 0f, _selectRight = 1f, _sliderBeforeLeft, _sliderBeforeRight;
    void Start()
    {
        GenerateTexture(512, 256);
        ApplySelectFromSliders();
    }

    void Update()
    {
        if (_selectLeft != SelectLeft || _selectRight != SelectRight)
        {
            _selectLeft = SelectLeft;
            _selectRight = SelectRight;
            ApplySelect();
        }
    }

    public void GenerateTexture(int width, int height)
    {
        _width = width;
        _heigth = height;
        
        int texSizeX = width, texSizeY = height;
        _texture = new Texture2D(texSizeX, texSizeY, TextureFormat.RGBA32, false);
        _texture.filterMode = FilterMode.Point;
        
        var clip = PulseBlockCenter.Instance.Clip;
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
        if (SliderLeft == null || SliderRight == null) return;

        if (_sliderBeforeLeft != SliderLeft.value)
        {
            SelectLeft = Mathf.Lerp(0f, _selectRight, SliderLeft.value);
            var otherVal = 1f - (_selectRight - SelectLeft) / (1 - SelectLeft);
            SliderRight.SetValueWithoutNotify(otherVal);
        }
        else if (_sliderBeforeRight != SliderRight.value)
        {
            SelectRight = Mathf.Lerp(_selectLeft, 1f, 1f - SliderRight.value);
            var otherVal = _selectLeft / SelectRight;
            SliderLeft.SetValueWithoutNotify(otherVal);
        }
        _sliderBeforeLeft = SliderLeft.value;
        _sliderBeforeRight = SliderRight.value;
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

    const float MinRate = 1000, MaxRate = 87000;
    public void Play()
    {
        AudioSource.clip = GetClip();
        UpdateVolume();
        AudioSource.Play();
    }

    AudioClip _clipCache;

    public AudioClip GetClip()
    {
        if (_clipCache != null) return _clipCache;
        var mainClip = PulseBlockCenter.Instance.Clip;
        var sampleStart = (int) (mainClip.samples * _selectLeft);
        var sampleAmount = (int) (mainClip.samples * _selectRight) - sampleStart;
        var sampleRate = (int) Mathf.Lerp(MinRate, MaxRate, 1 - SliderRate.value);
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
        AudioSource.volume = 1 - SliderVolume.value;
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