using System;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    public AudioSource KickSource;
    public AudioSource ClapSource;
    public AudioSource HatSource;

    static SoundsPlayer _instace;

    void OnEnable()
    {
        _instace = this;
    }

    public static void Kick()
    {
        _instace.KickSource.Play();
    }

    public static void Hat()
    {
        _instace.HatSource.Play();
    }

    public static void Clap()
    {
        _instace.ClapSource.Play();
        ColorPalette.SwitchToNextPalette();
    }
}