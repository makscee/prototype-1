using System;
using UnityEngine;

[Serializable]
public class BlockSerialized : JsonUtilitySerializable
{
    public int X, Y, PulseBlockX, PulseBlockY;

    public static bool Create(Block b, out BlockSerialized result)
    {
        result = null;
        if (!b.IsAnchored() || b.pulseBlock == null || b.X == b.pulseBlock.X && b.Y == b.pulseBlock.Y)
        {
            return false;
        }

        result = new BlockSerialized {X = b.X, Y = b.Y, PulseBlockX = b.pulseBlock.X, PulseBlockY = b.pulseBlock.Y};

        return true;
    }

    public void Deserialize()
    {
        Block.Create(X, Y, PulseBlockX, PulseBlockY);
    }
}