using System.Collections.Generic;
using UnityEngine;

public class WaveTextureProvider
{
    AudioClip _clip;
    float[] _data;
    static int chunkSize; // how many samples in pixel (samples / height)
    static int valueRange = 128;
    static int _width, _height;

    List<float[]> _chunks;

    public WaveTextureProvider(AudioClip clip, int width, int height)
    {
        _width = width;
        _height = height;
        valueRange = width;
        Clip = clip;
    }

    public AudioClip Clip
    {
        get => _clip;
        set
        {
            _clip = value;
            chunkSize = _clip.samples / _height;
            GenerateChunks();
        }
    }

    void GenerateChunks()
    {
        _data = new float[Clip.samples];
        Clip.GetData(_data, 0);
        _chunks = new List<float[]>(_height);
        for (var i = 0; i < _height; i++)
            _chunks.Add(new []{1f, -1f});
        for (var i = 0; i < _height * chunkSize; i++)
        {
            var chunkInd = i / chunkSize;
            _chunks[chunkInd][0] = Mathf.Min(_data[i], _chunks[chunkInd][0]);
            _chunks[chunkInd][1] = Mathf.Max(_data[i], _chunks[chunkInd][1]);
        }
    }

    public Texture GetTexture()
    {
        var texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false) {filterMode = FilterMode.Trilinear};
        var pixels = new Color32[_width * _height];
        for (var i = 0; i < _height; i++)
        {
            const int addWidth = 1;
            var from = Mathf.Clamp((int) ((_chunks[i][0] + 1) / 2 * (_width - 1)) - addWidth, 0, _width);
            var to = Mathf.Clamp((int) ((_chunks[i][1] + 1) / 2 * (_width - 1)) + addWidth, 0, _width);
            var onColor = new Color32(255, 255, 255, 255);
            const byte offVal = 0;
            var offColor = new Color32(offVal, offVal, offVal, offVal);
            for (var j = 0; j < _width; j++)
            {
                var color = j >= from && j < to ? onColor : offColor;
                pixels[Index(j, _height - (i + 1))] = color;
            }
        }
        texture.SetPixelData(pixels, 0);
        texture.Apply();
        return texture;
    }

    static int Index(int x, int y)
    {
        x = Mathf.Clamp(x, 0, _width - 1);
        y = Mathf.Clamp(y, 0, _height - 1);
        return x + _width * y;
    }
}