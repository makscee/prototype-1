using UnityEngine;

public class StaticAnchor : BindableAnchor
{
    public static IBindable Create(Vector2 position)
    {
        var anchor = new StaticAnchor();
        anchor._position = position;
        return anchor;
    }
    
    Vector2 _position;
    public override Vector2 GetPosition()
    {
        return _position;
    }
}