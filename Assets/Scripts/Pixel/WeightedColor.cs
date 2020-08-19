using UnityEngine;

public struct WeightedColor
{
    public Color Color;
    public float Weight;

    public WeightedColor(Color color, float weight)
    {
        Color = color;
        Weight = weight;
    }
    
    public static WeightedColor Clear = new WeightedColor(Color.clear, 0f);
}