using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    [SerializeField] private AudioClip loop;
    [SerializeField] private string[] soundEffectNames; //unoptimized version of a dictionary but unity hates dictionaries
    [SerializeField] private AudioClip[] soundEffectClips;
    private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();
    private AudioSource[] sources;
    private static AudioSourceManager instance = null;
    public static AudioSourceManager Instance { 
        get {return instance;} 
    }

    void Awake()
    {
        if (instance != null && instance != this) {
            instance.sources = instance.GetComponentsInChildren<AudioSource>();
            Destroy(this.gameObject);
            return;
        
        } else {
            instance = this;
        }

        for (int i = 0; i < soundEffectNames.Length; i++) soundEffects[soundEffectNames[i]] = soundEffectClips[i];

        sources = GetComponentsInChildren<AudioSource>();
        sources[0].clip = loop;

        playMusic();
        transform.parent = null;
        DontDestroyOnLoad(instance);
    }

    /// <summary>
    /// plays the looping music track
    /// </summary>
    void playMusic() {
        if (sources[0].isPlaying) {
            return;
        }
        sources[0].Play();
    }
    /// <summary>
    /// Plays a sound on a channel given the clip
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="channel"></param>
    /// <param name="volumeScale"></param>
    public void playSound(AudioClip clip, int channel = 1, float volumeScale = 1f)
    {
        sources[channel].PlayOneShot(clip, volumeScale);
    }

    /// <summary>
    /// Plays a sound on a channel given the name (assigned in prefab of Audio Player)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="channel"></param>
    /// <param name="volumeScale"></param>
    public void playSound(string name, int channel = -1, float volumeScale = 1f)
    {
        sources[channel].PlayOneShot(soundEffects[name], volumeScale);
    }

    
}
