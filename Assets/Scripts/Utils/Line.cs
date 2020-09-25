using UnityEngine;

public class Line
{
    public Vector2 From, To;
    public float Thickness;
    public Color32 Color;

    public Line(Color32 color, float thickness, Vector2 from, Vector2 to)
    {
        Color = color;
        Thickness = thickness;
        From = from;
        To = to;
    }
}