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
    [SerializeField] private float animationTime;
    [SerializeField] private LeanTweenType easeType;
    private float Yoffset;
    private int currentMenu;

    // Start is called before the first frame update
    void Start()
    {  
        currentMenu = 0;
        Yoffset = GetComponent<CanvasScaler>().referenceResolution.y;
    }

    void Update() {
        if ((Input.GetKeyDown(KeyCode.Escape) && !ManageGame.levelFinishing)) {
            if (currentMenu == 1) {
                changeMenu(0);
            }
            else {
                changeMenu(1);
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

    //menu animations
    public void changeMenu(int to) {
        
        //Debug.Log(currentMenu + "  " + to);

        die = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        UniversalAdditionalCameraData camData = cam.GetComponent<Camera>().GetComponent<UniversalAdditionalCameraData>();

        float target = (to > currentMenu) ? Yoffset : -Yoffset;

        if (to == 0) { //moving back to gameplay
            camData.renderPostProcessing = false;
            die.canControl = true;
            LeanTween.moveY(pause.GetComponent<RectTransform>(), pause.GetComponent<RectTransform>().position.x + target, animationTime).setEase(easeType);
        }
        else if (to == 1) { //moving to pause

            LeanTween.moveY(pause.GetComponent<RectTransform>(), 0, animationTime).setEase(easeType);

            if (currentMenu == 0) { //moving from gameplay
                camData.renderPostProcessing = false;
                die.canControl = true;
            }
            else { //moving from options
                LeanTween.moveY(options.GetComponent<RectTransform>(), pause.GetComponent<RectTransform>().position.x + target, animationTime).setEase(easeType);
            }
            
        }
        else { //moving to options
            LeanTween.moveY(pause.GetComponent<RectTransform>(), pause.GetComponent<RectTransform>().position.x + target, animationTime).setEase(easeType);
            LeanTween.moveY(options.GetComponent<RectTransform>(), 0, animationTime).setEase(easeType);
            
        }
        currentMenu = to;
    }

    
    
}
