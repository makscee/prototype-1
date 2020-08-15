using System.Collections.Generic;
using UnityEngine;

public class PixelPool
{
    Stack<Pixel> _pixelsPool = new Stack<Pixel>();

    public Pixel Get()
    {
        if (_pixelsPool.Count == 0)
            _pixelsPool.Push(Object.Instantiate(Prefabs.Instance.pixel).GetComponent<Pixel>());
        var pixel = _pixelsPool.Pop();
        pixel.gameObject.SetActive(true);
        return pixel;
    }

    public void Return(Pixel p)
    {
        p.StateReset();
        p.gameObject.SetActive(false);
        _pixelsPool.Push(p);
    }
}