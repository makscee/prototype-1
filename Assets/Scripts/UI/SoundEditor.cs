using UnityEngine;

public class SoundEditor : RootEditorScreen
{
    [SerializeField] SplitSlider volume, rate;
    int _direction;

    void Awake()
    {
        volume.onClick = Play;
        rate.onClick = Play;
        
        volume.onValueChange = v => Apply();
        rate.onValueChange = v => Apply();
    }
    public override void Select()
    {
        gameObject.SetActive(true);
    }

    public override void Deselect()
    {
        gameObject.SetActive(false);
    }

    public void SelectDirection(int dir)
    {
        _direction = dir;
        volume.value = rootEditor.RootBlock.soundsPlayer.Configs[dir].Volume;
        rate.value = (rootEditor.RootBlock.soundsPlayer.Configs[dir].Rate - rate.Min) / (rate.Max - rate.Min);
    }

    void Apply()
    {
        rootEditor.RootBlock.soundsPlayer.Configs[_direction].Volume = volume.value;
        rootEditor.RootBlock.soundsPlayer.Configs[_direction].Rate = rate.MultipliedValueInt;
    }
    
    void Play()
    {
        rootEditor.RootBlock.soundsPlayer.Play(_direction);
    }
}