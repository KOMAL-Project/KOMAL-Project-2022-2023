using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    /*
    Menu -1: Options
    Menu 0: Main Menu
    Menu 1: Level Selector
    ORDER DETERMINES DIRECTION THE MENU SCROLLS
    determineMenu method converts int to gameobject (may be a more efficent way to do this)
    */


    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject levelMenu;
    [SerializeField] private GameObject diceCamera;
    private DiceCameraScript camScript;
    [SerializeField] private float animationLength;
    [SerializeField] private float animationSpeed;
    [SerializeField] private float smoothDampTimeChange;
    [SerializeField] private float menuSpace;
    private Vector3 canvasPosition;
    private Vector2 higher;
    private Vector2 lower;
    

    
    private void Start() {

        canvasPosition = GetComponent<Transform>().position;
        higher = new Vector2(canvasPosition.x, canvasPosition.y + menuSpace);
        lower = new Vector2(canvasPosition.x, canvasPosition.y - menuSpace);
        camScript = diceCamera.GetComponent<DiceCameraScript>();

        StartCoroutine(menuEnter(mainMenu, true));
    }

   
    public void Quit() {
        Debug.Log("Game Quit");
        Application.Quit();
        
    }

    public void changeLevel(int level) {
        if (ManageGame.furthestLevel + 1 >= level) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level " + (level));
        }
    }

    public void changeMenu(int from, int to) {
        StartCoroutine(menuExit(determineMenu(from), from > to));
        StartCoroutine(menuEnter(determineMenu(to), to > from));
        if (from == 0 && to == 1) {
            camScript.pullDirection(animationLength, false);
        }
        else if (from == 1 && to == 0) {
            camScript.pullDirection(animationLength, true);
        }
        
    }

    private GameObject determineMenu(int identifier) {
        switch (identifier) {
            case 0: return mainMenu;
            case -1: return optionsMenu;
            case 1: return levelMenu;
            default: Debug.Log("THIS SHOULD NOT APPEAR EVER"); return new GameObject();
        }
    }


    private IEnumerator menuExit(GameObject menu, bool toTop) {

        Transform transform = menu.GetComponent<RectTransform>();
        Button[] buttons = GetComponentsInChildren<Button>();
        Vector2 target = toTop ? higher : lower;
        float animationTime = animationSpeed;

        foreach (var button in buttons) {
            button.enabled = false;
        }

        Vector2 velocity = Vector2.zero;
        float currentDuration = 0f;

        while (currentDuration <= animationLength) {
            transform.position = Vector2.SmoothDamp(transform.position, target, ref velocity, animationTime);
            animationTime -= smoothDampTimeChange;
            currentDuration += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        foreach (var button in buttons) {
            button.enabled = true;
        }
        menu.SetActive(false);

        
    }


    private IEnumerator menuEnter(GameObject menu, bool fromTop) {

        menu.SetActive(true);
        Transform transform = menu.GetComponent<RectTransform>();
        Button[] buttons = GetComponentsInChildren<Button>();
        float animationTime = animationSpeed;
        transform.position = fromTop ? higher : lower;
        

        foreach (var button in buttons) {
            button.enabled = false;
        }

        Vector2 velocity = Vector2.zero;
        float currentDuration = 0f;

        while (currentDuration <= animationLength) {
            transform.position = Vector2.SmoothDamp(transform.position, canvasPosition, ref velocity, animationTime);
            animationTime -= smoothDampTimeChange;
            currentDuration += Time.deltaTime;
            yield return null;
        }

        transform.position = canvasPosition;
        foreach (var button in buttons) {
            button.enabled = true;
        }
        
    }

    
}
