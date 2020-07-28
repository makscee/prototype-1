using UnityEngine;

public class SoundConfig
{
    float _selectFrom = 0.23f, _selectTo = 0.77f;

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
            const int rate = 44100;
            var newClip = ClipMaker.Make(clip, start, amount, rate);
            _clipCache = newClip;
            return newClip;
        }
    }
}