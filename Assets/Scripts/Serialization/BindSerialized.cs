using System;
using UnityEngine;

[Serializable]
public class BindSerialized : JsonUtilitySerializable
{
    public int FirstX, FirstY, SecondX, SecondY, Strength;

    public static bool Create(Bind b, out BindSerialized result)
    {
        result = null;
        if (!(b.First is Block block1 && b.Second is Block block2)) return false;
        result = new BindSerialized
        {
            FirstX = block1.logic.X,
            FirstY = block1.logic.Y,
            SecondX = block2.logic.X,
            SecondY = block2.logic.Y,
            Strength = b.Strength,
        };
        return true;
    }
}