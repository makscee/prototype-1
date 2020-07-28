using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundsPlayerSerialized : JsonUtilitySerializable
{
    public int PulseBlockX, PulseBlockY;
    public List<float> selectFrom = new List<float>(4);
    public List<float> selectTo = new List<float>(4);
    
    public static bool Create(SoundsPlayer soundsPlayer, int pulseBlockX, int pulseBlockY, out SoundsPlayerSerialized result)
    {
        result = new SoundsPlayerSerialized();
        result.PulseBlockX = pulseBlockX;
        result.PulseBlockY = pulseBlockY;
        for (var i = 0; i < 4; i++)
        {
            result.selectFrom.Add(soundsPlayer.Configs[i].SelectFrom);
            result.selectTo.Add(soundsPlayer.Configs[i].SelectTo);
        }

        return true;
    }

    public void Deserialize()
    {
        if (!FieldMatrix.Get(PulseBlockX, PulseBlockY, out var pb))
        {
            Debug.LogError($"No pulse block {PulseBlockX} {PulseBlockY}");
        }

        var pulseBlock = (PulseBlock) pb;
        for (var i = 0; i < 4; i++)
        {
            pulseBlock.SoundsPlayer.Configs[i].SelectFrom = selectFrom[i];
            pulseBlock.SoundsPlayer.Configs[i].SelectTo = selectTo[i];
        }
    }
}