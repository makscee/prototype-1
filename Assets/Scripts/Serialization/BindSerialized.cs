using System;

[Serializable]
public class BindSerialized : JsonUtilitySerializable
{
    public int FirstX, FirstY, SecondX, SecondY, Strength;
    public float OffsetX, OffsetY, BreakDistance, RopeLength;

    public static bool Create(Bind b, out BindSerialized result)
    {
        result = null;
        if (!(b.First is Block block1 && b.Second is Block block2) ||
            b.First.GetType() != typeof(Block) && b.Second.GetType() != typeof(Block)) return false;
        result = new BindSerialized
        {
            FirstX = block1.X,
            FirstY = block1.Y,
            SecondX = block2.X,
            SecondY = block2.Y,
            Strength = b.Strength,
            OffsetX = b.Offset.x,
            OffsetY = b.Offset.y,
            BreakDistance = b.BreakDistance,
            RopeLength = b.RopeLength
        };
        return true;
    }
}