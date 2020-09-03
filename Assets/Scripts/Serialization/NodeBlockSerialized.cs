using System;
using UnityEngine;

[Serializable]
public class NodeBlockSerialized : JsonUtilitySerializable
{
    public int X, Y, RootDir;

    public static bool Create(NodeBlock b, out NodeBlockSerialized result)
    {
        result = null;
        if (!b.IsAnchored)
        {
            return false;
        }

        result = new NodeBlockSerialized {X = b.logic.X, Y = b.logic.Y, RootDir = b.rootNum};

        return true;
    }

    public void Deserialize()
    {
        NodeBlock.Create(X, Y, RootDir, 0.1f);
    }
}