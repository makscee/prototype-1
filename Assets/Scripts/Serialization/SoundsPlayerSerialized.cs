using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundsPlayerSerialized : JsonUtilitySerializable
{
    public int dir;
    public List<int> selectFrom = new List<int>(4);
    public List<int> selectTo = new List<int>(4);
    public List<int> rate = new List<int>(4);
    public List<float> volume = new List<float>(4);
    
    public static bool Create(SoundsPlayer soundsPlayer, int dir, out SoundsPlayerSerialized result)
    {
        result = new SoundsPlayerSerialized();
        result.dir = dir;
        for (var i = 0; i < 4; i++)
        {
            result.selectFrom.Add(soundsPlayer.Configs[i].SelectFrom);
            result.selectTo.Add(soundsPlayer.Configs[i].SelectTo);
            result.rate.Add(soundsPlayer.Configs[i].Rate);
            result.volume.Add(soundsPlayer.Configs[i].Volume);
        }

        return true;
    }

    public void Deserialize()
    {
        var rootBlock = SharedObjects.Instance.rootBlocks[dir];
        for (var i = 0; i < 4; i++)
        {
            rootBlock.soundsPlayer.Configs[i].SelectFrom = selectFrom[i];
            rootBlock.soundsPlayer.Configs[i].SelectTo = selectTo[i];
            rootBlock.soundsPlayer.Configs[i].Rate = rate[i];
            rootBlock.soundsPlayer.Configs[i].Volume = volume[i];
        }
    }
}