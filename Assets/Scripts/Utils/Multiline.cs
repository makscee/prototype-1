using UnityEngine;

public class Multiline
{
    public Vector2[] Positions;
    public float Thickness;
    public Color32 Color;

    public Multiline(Color32 color, float thickness, Vector2[] positions)
    {
        Color = color;
        Thickness = thickness;
        Positions = positions;
    }
}