using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeReference] private bool mainMenuButton = true;
    [SerializeField] private int menuStart = 0;
    [SerializeField] private int menuDestination = 0;
    [SerializeField] private AudioClip sound;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GameObject.FindGameObjectWithTag("Audio").GetComponents<AudioSource>()[1];
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(click);

    }

    // Update is called once per frame
    public void playSound() {
        source.clip = sound;
        source.Play();
    }

    public void click() {
        playSound();
        
        if (mainMenuButton) {
            GetComponentInParent<MainMenuScript>().changeMenu(menuStart, menuDestination);
        }
    }



}
