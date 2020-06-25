using System;
using UnityEngine;

[Serializable]
public class BlockSerialized : JsonUtilitySerializable
{
    public int X, Y, PulseBlockX, PulseBlockY;

    public static bool Create(Block b, out BlockSerialized result)
    {
        result = null;
        if (!b.IsAnchored() || b.PulseBlock == null || b.X == b.PulseBlock.X && b.Y == b.PulseBlock.Y)
        {
            return false;
        }

        result = new BlockSerialized {X = b.X, Y = b.Y, PulseBlockX = b.PulseBlock.X, PulseBlockY = b.PulseBlock.Y};

        return true;
    }

    public void Deserialize()
    {
        Block.Create(X, Y, PulseBlockX, PulseBlockY);
    }
}