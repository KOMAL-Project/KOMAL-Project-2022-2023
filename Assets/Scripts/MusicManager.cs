using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip loop;
    
    private AudioSource source;
    private static MusicManager instance = null;
    public static MusicManager Instance { 
        get {return instance;} 
        }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        
        } else {
            instance = this;
        }

        source = GetComponent<AudioSource>();
        source.clip = loop;

        playMusic();
        DontDestroyOnLoad(instance);
    }

    void playMusic() {
        if (source.isPlaying) {
            return;
        }
        source.Play();
    }

    // Update is called once per frame
    
}
