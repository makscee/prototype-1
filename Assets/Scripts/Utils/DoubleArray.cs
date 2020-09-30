using System;
using UnityEngine;

public class DoubleArray<T>
{
    public T[] first, second;

    public void Merge()
    {
        var firstLength = first.Length;
        Array.Resize(ref first, firstLength + second.Length);
        Array.Copy(second, 0, first, firstLength, second.Length);
        second = new T[0];
    }

    public void Drop()
    {
        second = new T[0];
    }
    public T this[int ind] => ind < first.Length ? first[ind] : second[ind - first.Length];

    public int Length => first.Length + (second?.Length ?? 0);
}