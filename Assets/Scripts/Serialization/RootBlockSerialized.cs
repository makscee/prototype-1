using System;

[Serializable]
public class RootBlockSerialized : JsonUtilitySerializable
{
    public int X, Y, Id, ColorsId;

    public static bool Create(RootBlock b, out RootBlockSerialized result)
    {
        result = null;
        result = new RootBlockSerialized {X = b.logic.X, Y = b.logic.Y, Id = b.rootId, ColorsId = Roots.Root[b.rootId].palette.ColorsId};
        return true;
    }

    public void Deserialize()
    {
        Roots.CreateRoot(X, Y, Id, ColorsId);
    }
}