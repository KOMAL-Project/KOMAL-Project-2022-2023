using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LevelMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject cam;
    private DieController die;
    private DirectionalButtonController input;
    [SerializeField] private float animationLength;
    [SerializeField] private float animationSpeed;
    [SerializeField] private float smoothDampTimeChange;
    [SerializeField] private float menuSpace;
    private bool menuMoving;
    private Vector3 canvasPosition;
    private Vector2 higher;
    private Vector2 lower;
    private int menuType;

    // Start is called before the first frame update
    void Start()
    {
        canvasPosition = GetComponent<Transform>().position;
        higher = new Vector2(canvasPosition.x, canvasPosition.y + menuSpace);
        lower = new Vector2(canvasPosition.x, canvasPosition.y - menuSpace);
        menuType = 0;
        menuMoving = false;
        input = GameObject.FindGameObjectWithTag("D-Pad").GetComponent<DirectionalButtonController>();
    }

    void Update() {
        if (((Input.GetKeyDown(KeyCode.Escape) || input.keys["pause"]) && !ManageGame.levelFinishing) || (ManageGame.levelFinishing && (pause.activeInHierarchy || options.activeInHierarchy))) {
            if (!menuMoving) {
                changeActiveMenu(menuType == 0);
            }
        } 
        if (Input.GetKeyDown(KeyCode.R) && !ManageGame.levelFinishing) {
            restartLevel();
        }
    }

    public void restartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void returnToSelector() {
        SceneManager.LoadScene("Menu");
    }

    //up if menu is moving "up" a menu, like none to pause or pause to options/settings
    public void changeActiveMenu(bool up) {

        die = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        UniversalAdditionalCameraData camData = cam.GetComponent<Camera>().GetComponent<UniversalAdditionalCameraData>();
        

        if (menuType == 0) {
            menuType++;
            camData.renderPostProcessing = true;
            die.canControl = false;
            StartCoroutine(menuMove(pause, false, false));
        }
        else if (menuType == 1) {

            if (up) {
                menuType++;
                StartCoroutine(menuMove(pause, true, true));
                StartCoroutine(menuMove(options, false, false));
            }
            else {
                StartCoroutine(menuMove(pause, false, true));
                menuType--;
                camData.renderPostProcessing = false;
                die.canControl = true;
            }
            
        }
        else {
            menuType--;
            StartCoroutine(menuMove(pause, true, false));
            StartCoroutine(menuMove(options, false, true));
            
        }
    }

    private IEnumerator menuMove(GameObject menu, bool top, bool exit) {

        menu.SetActive(true);
        menuMoving = true;

        Transform transform = menu.GetComponent<RectTransform>();
        Button[] buttons = GetComponentsInChildren<Button>();
        //sets the location target to be the center piece if entering, otherwise above or below
        Vector3 target = !exit ? canvasPosition : (top ? higher : lower);

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
        menuMoving = false;

        //enables all buttons
        foreach (var button in buttons) {
            button.enabled = true;
        }

        //sets menu to inactive if it is now off screen
        if (exit) {menu.SetActive(false);}

    }

    
    
}
