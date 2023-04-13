using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{

    [SerializeField] private GameObject aboutMenu, tutorialMenu, optionsMenu, startMenu, levelMenu;
    [SerializeField] private float animationTime;
    [SerializeField] private LeanTweenType easeType;
    private float Yoffset;
    private float Xoffset;
    private int currentMenu;
    private int selectedChapter;
    private int selectedOptions;


    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start() {
        currentMenu = 0;
        selectedChapter = 1;

        CanvasScaler cs = GetComponentInParent<CanvasScaler>();
        Yoffset = cs.referenceResolution.y;
        Xoffset = cs.referenceResolution.x;

        LeanTween.moveY(startMenu,0,1.1f).setEase(LeanTweenType.easeOutBack);
    }

   
    public void Quit() {
        Debug.Log("Game Quit");
        Application.Quit();
        
    }

    /*
    For menus:
    options is -1 to -inf
    start is 0
    level is 1 to inf
    */

    private GameObject MenuLookup(int num) {
        switch (num) {
            case -3: return aboutMenu;
            case -2: return tutorialMenu;
            case -1: return optionsMenu;
            case 0: return startMenu;
            case 1: return levelMenu;

            default: Debug.Log("this should never appear (menuLookup returns incorrect obj)"); 
            return new GameObject();
    
        }
    }


    public void ChangeMenu(int to) {

        if (currentMenu >= 1 && to >= 1) { //for moving between chapters

            selectedChapter = to;
            
            float target = (to > currentMenu) ? -Xoffset : Xoffset;

            RectTransform leveltransform = levelMenu.GetComponent<RectTransform>();
            LeanTween.moveX(leveltransform, leveltransform.localPosition.x + target, animationTime).setEase(easeType);

        }
        
        else { //for moving between menus

            float target = (to > currentMenu) ? -Yoffset : Yoffset;

            if (currentMenu > 1) {
                currentMenu = 1;
            }

            LeanTween.moveY(MenuLookup(currentMenu).GetComponent<RectTransform>(), target, animationTime).setEase(easeType);
            LeanTween.moveY(MenuLookup(to).GetComponent<RectTransform>(), 0, animationTime).setEase(easeType);

        }

        //Debug.Log(currentMenu + "  " + to + "  " + selectedChapter);

        if (to == 1) { //changes menu to chapter if moving to level menu
            currentMenu = selectedChapter;
        }
        else { //changes current menu to actual menu
            currentMenu = to;
        }
        //Debug.Log(currentMenu);

        return;
    }

    
}
