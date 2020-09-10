using System;
using UnityEngine;

[Serializable]
public class NodeBlockSerialized : JsonUtilitySerializable
{
    public int X, Y, RootId;

    public static bool Create(NodeBlock b, out NodeBlockSerialized result)
    {
        result = null;
        if (!b.IsAnchored)
        {
            return false;
        }

        result = new NodeBlockSerialized {X = b.logic.X, Y = b.logic.Y, RootId = b.rootId};

        return true;
    }

    public void Deserialize()
    {
        NodeBlock.Create(X, Y, RootId, 0.1f);
    }
}