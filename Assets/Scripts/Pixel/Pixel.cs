using System;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public int X, Y;
    
    [SerializeField] SpriteRenderer _spriteRenderer;

    public Pixel StateReset()
    {
        X = 0;
        Y = 0;
        _spriteRenderer.color = Color.magenta;
        SetShadow(false);
        return this;
    }

    public Pixel SetColor(Color c)
    {
        _spriteRenderer.color = c;
        return this;
    }

    public Pixel Move(int x, int y)
    {
        X = x;
        Y = y;
        transform.position = new Vector3(x, y);
        return this;
    }

    public Pixel SetShadow(bool value)
    {
        gameObject.layer = value ? 12 : 0;
        return this;
    }
}