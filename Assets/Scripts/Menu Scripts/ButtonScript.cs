using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private string buttonType;
    [SerializeField] private int menuDestination = 0;
    [SerializeField] private AudioClip sound;
    private AudioSource source;
    private MainMenuScript mmm;
    // Start is called before the first frame update
    void Start()
    {
        source = GameObject.FindGameObjectWithTag("Audio").GetComponents<AudioSource>()[1];
        mmm = GetComponentInParent<MainMenuScript>();
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(click);

    }

    public void playSound() {
        source.clip = sound;
        source.Play();
    }

    public void click() {
        playSound();
        mmm.changeMenu(menuDestination);
        

    }


}
