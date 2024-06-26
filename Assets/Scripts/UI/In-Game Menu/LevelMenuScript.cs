using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject options;
    private DieController die;
    [SerializeField] private float animationTime;
    [SerializeField] private LeanTweenType easeType;
    private float Yoffset;
    private int currentMenu;
    ManageGame gm;

    // Start is called before the first frame update
    void Start()
    {  
        currentMenu = 0;
        //Debug.Log(Yoffset);
        Yoffset = GetComponent<CanvasScaler>().referenceResolution.y;
        die = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        gm = ManageGame.Instance;
    }

    void Update() 
    {
        //this is all old stuff from pc game
        /*
        if ((Input.GetKeyDown(KeyCode.Escape) && !ManageGame.levelFinishing)) 
        {
            if (currentMenu == 1) 
            {
                Debug.Log("Out of Menu");
                changeMenu(0);
                gm.frc.lockToFast = gm.containsPipCharges;
                gm.frc.SetFullFPSTime(1);
            }
            else 
            {
                Debug.Log("Into Menu");
                changeMenu(1);
                gm.frc.lockToFast = true;
            }
        } 
        if (Input.GetKeyDown(KeyCode.R) && !ManageGame.levelFinishing) 
        {
            restartLevel();
        }
        */
    }

    public void restartLevel() 
    {
        ButtonScript.moving = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void returnToSelector() 
    {
        ButtonScript.moving = false;
        SceneManager.LoadScene("Menu");
    }

    public void ChangeMenu(int to) 
    {

        if (ManageGame.levelFinishing || to == -1) 
        {
            return;
        }

        // Freeze die motion if pausing, unfreeze if unpausing:
        die.canControl = to == 0;
        // pause frame rate throttle if entering menu.
        gm.frc.lockToFast = to != 0 ? true: gm.containsPipCharges;
        gm.frc.SetFullFPSTime(1);

        //Debug.Log(currentMenu + "  " + to);

        die = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();

        float target = (to > currentMenu) ? Yoffset : -Yoffset;

        LTDescr tweenAction;
        ButtonScript.moving = true;

        if (to == 0) 
        { //moving back to gameplay
            //camData.renderPostProcessing = false;
            tweenAction = LeanTween.moveY(pause.GetComponent<RectTransform>(), pause.GetComponent<RectTransform>().localPosition.y + target, animationTime).setEase(easeType);
        }
        else if (to == 1) 
        { //moving to pause

            tweenAction = LeanTween.moveY(pause.GetComponent<RectTransform>(), 0, animationTime).setEase(easeType);

            if (currentMenu == 0)
            { //moving from gameplay
            }
            else 
            { //moving from options
                LeanTween.moveY(options.GetComponent<RectTransform>(), options.GetComponent<RectTransform>().localPosition.y + target, animationTime).setEase(easeType);
                SaveDataManager.setPlayerPrefs();
            }
            
        }
        else 
        { //moving to options
            tweenAction = LeanTween.moveY(pause.GetComponent<RectTransform>(), pause.GetComponent<RectTransform>().localPosition.y + target, animationTime).setEase(easeType);
            LeanTween.moveY(options.GetComponent<RectTransform>(), 0, animationTime).setEase(easeType);
            
        }

        tweenAction.setOnComplete(() => {
            ButtonScript.moving = false;
            });

        currentMenu = to;
    }
    
}
