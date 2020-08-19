using System;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public int X, Y;
    
    [SerializeField] SpriteRenderer _spriteRenderer;

    void Start()
    {
        transform.position = new Vector3(X, Y);
    }

    void SetColor(Color c)
    {
        _spriteRenderer.color = c;
    }

    void Update()
    {
        SetColor(PixelDriver.GetColor(X, Y));
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