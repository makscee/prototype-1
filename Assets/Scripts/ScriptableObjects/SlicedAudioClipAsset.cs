using UnityEngine;

[CreateAssetMenu(fileName = "NewSlicedAudioClipAsset", menuName = "ScriptableObjects/SlicedAudioClipAsset")]
public class SlicedAudioClipAsset : ScriptableObject
{
    public AudioClip audioClip;
    public int[] slices;
}