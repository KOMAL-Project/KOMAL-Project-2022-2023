using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private string buttonType;
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

    public void playSound() {
        source.clip = sound;
        source.Play();
    }

    public void click() {
        playSound();
        animator.SetTrigger(animationTrigger);
        //Change int after trigger is activated
        StartCoroutine(changeCurrentMenu());

    }

    private IEnumerator changeCurrentMenu() {
         yield return new WaitForSeconds(0.05f);
         animator.SetInteger("Current Menu", menuDestination);
    }



}
