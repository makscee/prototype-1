using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSerialized : JsonUtilitySerializable
{
    public List<BlockSerialized> Blocks;
    public List<BindSerialized> Binds;
    public List<SoundsPlayerSerialized> SoundsPlayers;

    public static GameSerialized Create()
    {
        var result = new GameSerialized
        {
            Blocks = new List<BlockSerialized>(),
            Binds = new List<BindSerialized>(),
            SoundsPlayers = new List<SoundsPlayerSerialized>(),
        };
        foreach (var block in FieldMatrix.GetAllAsList())
            if (BlockSerialized.Create(block, out var t))
                result.Blocks.Add(t);
        foreach (var bind in BindMatrix.GetAllAsList())
            if (BindSerialized.Create(bind, out var t))
                result.Binds.Add(t);
        for (var i = 0; i < 4; i++)
        {
            var pb = PulseBlockCenter.Instance.PulseBlocks[i];
            if (SoundsPlayerSerialized.Create(pb.SoundsPlayer, pb.X, pb.Y, out var t)) 
                result.SoundsPlayers.Add(t);
        }
        return result;
    }

    public static GameSerialized Create(string json)
    {
        return JsonUtility.FromJson<GameSerialized>(json);
    }

    public void Deserialize()
    {
        foreach (var bs in Blocks)
        {
            bs.Deserialize();
        }

        foreach (var bs in Binds)
        {
            FieldMatrix.Get(bs.FirstX, bs.FirstY, out var first);
            FieldMatrix.Get(bs.SecondX, bs.SecondY, out var second);
            var offset = new Vector2(bs.SecondX - bs.FirstX, bs.SecondY - bs.FirstY);
            BindMatrix.AddBind(first, second, offset, bs.Strength, bs.RopeLength, bs.BreakDistance);
        }

        foreach (var sps in SoundsPlayers)
        {
            sps.Deserialize();
        }
    }
}