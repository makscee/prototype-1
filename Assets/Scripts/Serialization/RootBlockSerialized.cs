using System;
using UnityEngine;

[Serializable]
public class RootBlockSerialized : JsonUtilitySerializable
{
    public int X, Y, Dir;

    public static bool Create(RootBlock b, out RootBlockSerialized result)
    {
        result = null;
        if (!b.IsAnchored)
        {
            return false;
        }

        result = new RootBlockSerialized {X = b.logic.X, Y = b.logic.Y, Dir = b.direction};

        return true;
    }

    public void Deserialize()
    {
        RootBlock.Create(X, Y, Dir);
    }
}