using UnityEngine;

public class StaticAnchor : BindableAnchor
{
    public static IBindable Create(Vector2 position, bool isAnchor = true)
    {
        var anchor = new StaticAnchor();
        anchor._position = position;
        anchor._isAnchor = isAnchor;
        return anchor;
    }

    bool _isAnchor;
    public override bool IsAnchor()
    {
        return _isAnchor;
    }

    Vector2 _position;
    public override Vector2 GetPosition()
    {
        return _position;
    }
}