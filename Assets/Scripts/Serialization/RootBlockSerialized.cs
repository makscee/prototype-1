using System;

[Serializable]
public class RootBlockSerialized : JsonUtilitySerializable
{
    public int X, Y, Id, ColorsId;

    public static bool Create(RootBlock b, out RootBlockSerialized result)
    {
        result = null;
        if (!b.IsAnchored)
        {
            return false;
        }

        result = new RootBlockSerialized {X = b.logic.X, Y = b.logic.Y, Id = b.rootNum, ColorsId = Roots.Palettes(b.rootNum).ColorsId};

        return true;
    }

    public void Deserialize()
    {
        RootBlock.Create(X, Y, Id, ColorsId); 
    }
}