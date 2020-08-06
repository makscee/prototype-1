using UnityEngine;

public class SoundConfig
{
    float _selectFrom = 0.23f, _selectTo = 0.77f, _volume;
    int _rate;

    public float SelectFrom
    {
        get => _selectFrom;
        set
        {
            _selectFrom = value;
            _clipCache = null;
        }
    }
    public float SelectTo
    {
        get => _selectTo;
        set
        {
            _selectTo = value;
            _clipCache = null;
        }
    }
    public int Rate
    {
        get => _rate;
        set
        {
            _rate = value;
            _clipCache = null;
        }
    }
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            _clipCache = null;
        }
    }

    AudioClip _clipCache;
    public AudioClip Clip
    {
        get
        {
            if (_clipCache != null) return _clipCache;
            var clip = WaveEditor.Instance.clip;
            var start = (int)(SelectFrom * clip.samples);
            var amount = (int) ((SelectTo - SelectFrom) * clip.samples);
            if (amount == 0) return null;
            var newClip = ClipMaker.Make(clip, start, amount, Rate);
            _clipCache = newClip;
            return newClip;
        }
    }
}