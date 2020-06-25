using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public enum InterpolationType
{
    Square, InvSquare, Linear
}

[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public class Interpolator<T> : IUpdateable
{
    float _over, _t;
    T _from, _to, _cur;
    Dictionary<Type, int> _types = new Dictionary<Type, int>();
    Func<T, T, T> _addFunc, _subtractFunc;
    Func<T, float, T> _multiplyFunc;
    public Interpolator(T from, T to, float over, Func<T, float, T> multiplyFunc, Func<T, T, T> addFunc, Func<T, T, T> subtractFunc)
    {
        _from = from;
        _cur = from;
        _to = to;
        _over = over;
        _multiplyFunc = multiplyFunc;
        _addFunc = addFunc;
        _subtractFunc = subtractFunc;
    }
    
    
    Action<T> _passDelta;
    public Interpolator<T> PassDelta(Action<T> action)
    {
        _passDelta = action;
        return this;
    }

    Action<T> _passValue;
    public Interpolator<T> PassValue(Action<T> action)
    {
        _passValue = action;
        return this;
    }

    InterpolationType _interpolationType = InterpolationType.Linear;
    public Interpolator<T> Type(InterpolationType type)
    {
        _interpolationType = type;
        return this;
    }

    bool _isDone;
    public void Update()
    {
        var before = _cur;
        var tUnit = _t / _over;

        switch (_interpolationType)
        {
            case InterpolationType.Linear:
                break;
            case InterpolationType.Square:
                tUnit *= tUnit;
                break;
            case InterpolationType.InvSquare:
                tUnit = 1 - (1 - tUnit) * (1 - tUnit);
                break;
        }
        
        _cur = _addFunc(_from, _multiplyFunc(_subtractFunc(_to, _from), tUnit));
        _passDelta?.Invoke(_subtractFunc(_cur, before));
        _passValue?.Invoke(_cur);
        if (_t == _over)
        {
            _isDone = true;
            return;
        }

        _t += Time.deltaTime;
        if (_t > _over)
        {
            _t = _over;
        }
    }

    public bool IsDone()
    {
        return _isDone;
    }
}