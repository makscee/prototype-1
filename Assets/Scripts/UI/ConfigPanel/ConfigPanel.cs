using System;
using UnityEngine;

public class ConfigPanel : MonoBehaviour
{
    int _direction;
    public SplitSlider rate, volume;

    void Start()
    {
        _direction = GetComponentInParent<DirectionIdHolder>().id;
        GameManager.OnNextFrame += () =>
        {
            var rootBlock = Roots.Root[GetComponentInParent<RootIdHolder>().id].block;
            var soundsPlayer = rootBlock.soundsPlayer;
            var config = soundsPlayer.Configs[_direction]; 
            rate.InitValue(config.Rate);
            volume.InitValue(config.Volume * 100);
            rate.onValueChange += v =>
                config.Rate = Mathf.RoundToInt(v);
            volume.onValueChange += v =>
                config.Volume = v / 100;

            rate.onClick += () => soundsPlayer.Play(_direction);
            volume.onClick += () => soundsPlayer.Play(_direction);
        };
    }
}