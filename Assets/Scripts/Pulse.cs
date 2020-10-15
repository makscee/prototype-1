using System;
using UnityEngine;

public class Pulse
{
    float _bpm;
    float _tickTime;

    public Pulse()
    {
        Bpm = 120;
    }

    public float Bpm
    {
        get => _bpm;
        set
        {
            _bpm = value;
            _tickTime = 60f / _bpm / 4;
        }
    }

    Action _subscribers;
    public void SubscribeToNext(Action a)
    {
        _subscribers += a;
    }

    public void UnsubscribeFromNext(Action a)
    {
        _subscribers -= a;
    }


    float _lastT;
    public void Update()
    {
        var t = Time.time - Mathf.Floor(Time.time / _tickTime) * _tickTime;
        if (_lastT > t) DoPulse();
        _lastT = t;
    }

    void DoPulse()
    {
        var t = _subscribers;
        _subscribers = null;
        t?.Invoke();
    }
}