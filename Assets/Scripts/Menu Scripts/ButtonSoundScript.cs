using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundScript : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GameObject.FindGameObjectWithTag("Audio").GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    public void playSound() {
        source.clip = sound;
        source.Play();
    }

}
