using System;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public int X, Y;
    
    [SerializeField] SpriteRenderer spriteRenderer;
    Color _current;

    void Start()
    {
        transform.position = new Vector3(X, Y);
    }

    void SetColor(Color c)
    {
        spriteRenderer.color = c;
        _current = c;
    }

    void Update()
    {
        var c = PixelDriver.GetColor(X, Y);
        if (c == _current) return;
        SetColor(c);
    }

    void OnDisable()
    {
        Destroy(gameObject);
    }

    public static Pixel Create(int x, int y)
    {
        var p = Instantiate(Prefabs.Instance.pixel).GetComponent<Pixel>();
        p.X = x;
        p.Y = y;
        return p;
    }
}