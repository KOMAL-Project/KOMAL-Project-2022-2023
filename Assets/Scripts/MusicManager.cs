using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip start;
    [SerializeField] private AudioClip loop;
    
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = start;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying) {
            source.clip = loop;
            source.loop = true;
            source.Play();
        }
    }
}
