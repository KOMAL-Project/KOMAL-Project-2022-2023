using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip loop;
    public AudioSource[] sources;
    private static MusicManager instance = null;
    public static MusicManager Instance { 
        get {return instance;} 
        }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        
        } else {
            instance = this;
        }

        sources = GetComponentsInChildren<AudioSource>();
        sources[0].clip = loop;

        playMusic();
        DontDestroyOnLoad(instance);
    }

    void playMusic() {
        if (sources[0].isPlaying) {
            return;
        }
        sources[0].Play();
    }

    // Update is called once per frame
    
}
