using UnityEngine;

[CreateAssetMenu(fileName = "slicedClip", menuName = "ScriptableObjects/SlicedAudioClipAsset")]
public class SlicedAudioClipAsset : ScriptableObject
{
    public AudioClip audioClip;
    public int[] slices;
}