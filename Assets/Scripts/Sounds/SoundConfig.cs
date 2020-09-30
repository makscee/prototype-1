using UnityEngine;

public class SoundConfig
{
    int _selectFrom = 0, _selectTo = 3000, _rate = 44100;

    public int SelectFrom
    {
        get => _selectFrom;
        set
        {
            _selectFrom = value;
            _clipCache = null;
        }
    }
    public int SelectTo
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
    public float Volume { get; set; } = 1f;

    readonly SoundsPlayer _player;
    public SoundConfig(SoundsPlayer player)
    {
        _player = player;
    }
    public void CutoutAdjust(int from, int to)
    {
        var length = to - from;
        if (from > _selectTo) return;
        if (_selectFrom >= to)
        {
            _selectFrom -= length;
            _selectTo -= length;
            return;
        }

        if (from <= _selectFrom && to >= _selectTo)
        {
            _selectFrom = from;
            _selectTo = from;
            return;
        }
        
        if (_selectFrom <= from && _selectTo >= to)
        {
            _selectTo -= length;
            return;
        }

        if (from < _selectFrom && _selectFrom < to)
        {
            _selectFrom = from;
            _selectTo -= length;
            return;
        }

        if (from < _selectTo && _selectTo < to)
        {
            _selectTo = from;
            return;
        }
    }

    AudioClip _clipCache;
    public AudioClip Clip
    {
        get
        {
            if (_clipCache != null) return _clipCache;
            var clip = _player.SlicedClip.audioClip;
            var amount = SelectTo - SelectFrom;
            if (amount == 0) return null;
            var newClip = ClipMaker.Make(clip, _selectFrom, _selectTo, Rate);
            _clipCache = newClip;
            return newClip;
        }
    }
}