using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeReference] private bool mainMenuButton = true;
    [SerializeField] private int menuDestination = 0;
    [SerializeField] private AudioClip sound;
    [SerializeField] private string animationTrigger;
    private AudioSource source;
    private Animator animator;
    private MainMenuScript mmm;
    // Start is called before the first frame update
    void Start()
    {
        source = GameObject.FindGameObjectWithTag("Audio").GetComponents<AudioSource>()[1];
        animator = GetComponentInParent<Animator>();
        mmm = GetComponentInParent<MainMenuScript>();
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(click);

    }

    // Update is called once per frame
    public void playSound() {
        source.clip = sound;
        source.Play();
    }

    public void click() {
        playSound();
        animator.SetTrigger(animationTrigger);
        StartCoroutine(changeCurrentMenu());

        /*
        if (mainMenuButton) {
            mmm.changeMenu(menuStart, menuDestination);
        }
        else if (menuDestination != menuStart){
            GetComponentInParent<LevelMenuScript>().changeActiveMenu(menuDestination > menuStart);
        }
        */
    }

    private IEnumerator changeCurrentMenu() {
         yield return new WaitForSeconds(0.05f);
         animator.SetInteger("Current Menu", menuDestination);
    }



}
