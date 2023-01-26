using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private string buttonType;
    [SerializeField] private int menuDestination = 0;
    [SerializeField] private AudioClip sound;
    private AudioSource source;
    private MainMenuScript mms;
    private LevelMenuScript lms;
    // Start is called before the first frame update
    void Start()
    {
        source = GameObject.FindGameObjectWithTag("Audio").GetComponents<AudioSource>()[1];
        mms = GetComponentInParent<MainMenuScript>();
        lms = GetComponentInParent<LevelMenuScript>();
        
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(click);
    }

    public void playSound() {
        source.PlayOneShot(sound, 1.0f);
    }

    public void click() {
        playSound();
        if (mms == null) { // in-level menu
            lms.changeMenu(menuDestination);
        }
        else { //main menu
            mms.ChangeMenu(menuDestination);
        }
    }
}