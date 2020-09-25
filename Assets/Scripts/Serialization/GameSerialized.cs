using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSerialized : JsonUtilitySerializable
{
    public List<NodeBlockSerialized> NodeBlocks;
    public List<RootBlockSerialized> RootBlocks;
    public List<BindSerialized> Binds;
    public List<SoundsPlayerSerialized> SoundsPlayers;

    public static GameSerialized Create()
    {
        var result = new GameSerialized
        {
            NodeBlocks = new List<NodeBlockSerialized>(),
            RootBlocks = new List<RootBlockSerialized>(),
            Binds = new List<BindSerialized>(),
            SoundsPlayers = new List<SoundsPlayerSerialized>(),
        };
        foreach (var block in FieldMatrix.GetAllAsList())
            switch (block)
            {
                case NodeBlock nodeBlock:
                {
                    if (NodeBlockSerialized.Create(nodeBlock, out var t))
                        result.NodeBlocks.Add(t);
                    break;
                }
                case RootBlock rootBlock:
                {
                    if (RootBlockSerialized.Create(rootBlock, out var t))
                        result.RootBlocks.Add(t);
                    break;
                }
            }
        foreach (var bind in BindMatrix.GetAllAsList()) 
            if (BindSerialized.Create(bind, out var t))
                result.Binds.Add(t);
        foreach (var rootBlock in Roots.Blocks.Values)
            if (rootBlock != null && SoundsPlayerSerialized.Create(rootBlock.soundsPlayer, rootBlock.rootId, out var t))
                result.SoundsPlayers.Add(t);
        return result;
    }

    public static GameSerialized Create(string json)
    {
        return JsonUtility.FromJson<GameSerialized>(json);
    }

    public void Deserialize()
    {
        foreach (var rb in RootBlocks)
            rb.Deserialize();
        foreach (var bs in NodeBlocks)
            bs.Deserialize();

        foreach (var bs in Binds)
        {
            FieldMatrix.Get(bs.FirstX, bs.FirstY, out var first);
            FieldMatrix.Get(bs.SecondX, bs.SecondY, out var second);
            var offset = new Vector2(bs.SecondX - bs.FirstX, bs.SecondY - bs.FirstY);
            BindMatrix.AddBind(first, second, offset, bs.Strength);
        }

        foreach (var sps in SoundsPlayers)
        {
            sps.Deserialize();
        }
    }
}