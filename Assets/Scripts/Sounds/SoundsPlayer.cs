using System;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    public AudioSource[] AudioSources = new AudioSource[4];

    public WaveModule[] WaveModules = new WaveModule[4];
    public GameObject[] ConfigRacks = new GameObject[4];

    void OnEnable()
    {
        GameManager.InvokeAfterServiceObjectsInitialized(CreateConfigRacks);
        GameManager.InvokeAfterServiceObjectsInitialized(Init);
    }

    void Init()
    {
        if (WaveModules[0] == null) return;
        
        for (var i = 0; i < 4; i++)
        {
            var newSource = gameObject.AddComponent<AudioSource>(); 
            newSource.playOnAwake = false;
            newSource.clip = WaveModules[i].GetClip();
            newSource.volume = WaveModules[i].AudioSource.volume;
            AudioSources[i] = newSource;
        }
    }

    void CreateConfigRacks()
    {
        for (var i = 0; i < 4; i++)
        {
            var configRack = Instantiate(Prefabs.Instance.ConfigRack, SharedObjects.Instance.UICanvas.transform);
            configRack.transform.position = transform.position + (Vector3)Utils.CoordsFromDir(i) * 2;
            var waveModule = configRack.transform.GetComponentInChildren<WaveModule>();
            WaveModules[i] = waveModule;
            ConfigRacks[i] = configRack;
            configRack.SetActive(false);
        }
    }

    public void ConfigRacksSetActive(bool value)
    {
        foreach (var configRack in ConfigRacks)
        {
            configRack.SetActive(value);
        }
    }

    public void ConfigRacksAnimatedSetActive(bool value, float over = 0.15f)
    {
        foreach (var configRack in ConfigRacks)
        {
            if (value) configRack.SetActive(true);
            else Animator.Invoke(() => configRack.SetActive(false)).In(over);
            configRack.transform.localScale = value ? Vector3.zero : Vector3.one;
            Animator.Interpolate(0f, 1f, over)
                .PassDelta(v =>
                {
                    v = value ? v : -v;
                    configRack.transform.localScale += new Vector3(v, v, v);
                }).Type(InterpolationType.InvSquare);
        }
    }

    public bool IsConfigRacksShown()
    {
        return ConfigRacks[0].activeInHierarchy;
    }
    void Update()
    {
        
    }

    public void Play(int dir)
    {
        AudioSources[dir].clip = WaveModules[dir].GetClip();
        AudioSources[dir].volume = WaveModules[dir].AudioSource.volume;
        AudioSources[dir].Play();
    }

    void OnDisable()
    {
        foreach (var audioSource in AudioSources)
        {
            Destroy(audioSource);
        }
    }
}