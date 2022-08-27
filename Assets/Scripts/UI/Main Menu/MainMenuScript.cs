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
    private Animator animator;
    

    
    private void Start() {

        canvasPosition = GetComponent<Transform>().position;
        higher = new Vector2(canvasPosition.x, canvasPosition.y + menuSpace);
        lower = new Vector2(canvasPosition.x, canvasPosition.y - menuSpace);
        camScript = diceCamera.GetComponent<DiceCameraScript>();

        StartCoroutine(menuMove(mainMenu, true, false));
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
        StartCoroutine(menuMove(determineMenu(from), from > to, true));
        StartCoroutine(menuMove(determineMenu(to), to > from, false));

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


    private IEnumerator menuMove(GameObject menu, bool top, bool exit) {

        menu.SetActive(true);

        Transform transform = menu.GetComponent<RectTransform>();
        Button[] buttons = GetComponentsInChildren<Button>();

        //sets the location target to be the center piece if entering, otherwise above or below
        Vector2 target = !exit ? canvasPosition : (top ? higher : lower);

        //sets starting position to higher or lower if entering
        if (!exit) {transform.position = (top) ? higher : lower;}

        //disables all buttons
        foreach (var button in buttons) {
            button.enabled = false;
        }

        Vector2 velocity = Vector2.zero;
        float currentDuration = 0f;
        float animationTime = animationSpeed;

        while (currentDuration <= animationLength) {
            transform.position = Vector2.SmoothDamp(transform.position, target, ref velocity, animationTime);
            //speeds up animation time slightly to ensure menu reaches it's destination
            animationTime -= smoothDampTimeChange;
            currentDuration += Time.deltaTime;
            yield return null;
        }

        transform.position = target;

        //enables all buttons
        foreach (var button in buttons) {
            button.enabled = true;
        }

        //sets menu to inactive if it is now off screen
        if (exit) {menu.SetActive(false);}

    }

    
}
