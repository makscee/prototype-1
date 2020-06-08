using System;
using UnityEngine;

public static class GlobalPulse
{
    const float Bpm = 70;
    static readonly float TickTime;

    static GlobalPulse()
    {
        TickTime = 60f / Bpm / 4;
    }
    
    static Action _subscribers;
    public static void SubscribeToNext(Action a)
    {
        _subscribers += a;
    }

    public static void UnsubscribeFromNext(Action a)
    {
        _subscribers -= a;
    }


    static float _t;
    public static void Update()
    {
        _t += Time.deltaTime;
        if (_t > TickTime)
        {
            _t -= TickTime;
            Pulse();
        }
    }

    static void Pulse()
    {
        var t = _subscribers;
        _subscribers = null;
        t?.Invoke();
    }
}