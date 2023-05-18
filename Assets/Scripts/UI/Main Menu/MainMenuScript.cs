using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{

    [SerializeField] private GameObject aboutMenu, tutorialMenu, optionsMenu, startMenu, levelMenu, creditsBg;
    [SerializeField] private Animator credits;
    [SerializeField] private float animationTime;
    [SerializeField] private LeanTweenType easeType;
    [SerializeField] private List<GameObject> levelMenus, tutorialMenus;
    public static float Xoffset, Yoffset; 
    private int currentMenu, selectedChapter, selectedTutorial;
    [SerializeField] Camera backgroundCam;
    [SerializeField] Color mainColor = new Color(0.29f, 0.84f, 0.84f, 0f);
    [SerializeField] ColorPalette[] palettes;
    private void Start() 
    {
        OnDemandRendering.renderFrameInterval = 1;

        currentMenu = 0;
        selectedChapter = 1;
        selectedTutorial = -3;

        Yoffset = GetComponent<RectTransform>().rect.y * -2;
        Xoffset = GetComponent<RectTransform>().rect.x * -2;

        //offsetting each menu vertically
        Vector3 below = new Vector3(0, -Yoffset, 0);
        Vector3 above = new Vector3(0, Yoffset, 0);

        aboutMenu.transform.localPosition = below;
        tutorialMenu.transform.localPosition = below;
        optionsMenu.transform.localPosition = below;
        levelMenu.transform.localPosition = above;
        startMenu.transform.localPosition = above;    
        
        //offsetting level menus and tutorial panels horizontally - hardcoded, can be changed
        for (int i = 0; i < levelMenus.Count; i++) {levelMenus[i].transform.localPosition = new Vector3(Xoffset * i, 0, 0);};
        for (int i = 0; i < tutorialMenus.Count; i++) {tutorialMenus[i].transform.localPosition = new Vector3(Xoffset * i, 0, 0);}


        LeanTween.moveY(startMenu,0,1.1f).setEase(LeanTweenType.easeOutBack);
        credits = GameObject.FindGameObjectWithTag("Credits").GetComponent<Animator>();
        creditsBg = GameObject.FindGameObjectWithTag("Credits").transform.parent.GetChild(0).gameObject;
        creditsBg.GetComponent<Image>().enabled = false;

        backgroundCam.backgroundColor = mainColor;
    }


    public void Quit() {
        Debug.Log("Game Quit");
        Application.Quit();
        
    }

    /*
    For menus:
    tutorials are -3 to -inf
    about is -2
    options is -1
    start is 0
    level is 1 to inf
    */

    private GameObject MenuLookup(int num) 
    {
        switch (num) 
        {
            case -3: return tutorialMenu;
            case -2: return aboutMenu;
            case -1: return optionsMenu;
            case 0: return startMenu;
            case 1: return levelMenu;

            default: Debug.Log("this should never appear (menuLookup returns incorrect obj)"); 
            return new GameObject();
    
        }
    }


    public void ChangeMenu(int to) 
    {

        //Debug.Log("" + currentMenu + " " + to);

        // controls the credits in the about menu
        if (to == -2)
        {
            credits.Play("Roll");
            creditsBg.GetComponent<Image>().enabled = true;
        }
        else
        {
            credits.Play("Initial State");
            StartCoroutine(TurnOffAboutBG());
        }

        if (currentMenu >= 1 && to >= 1) 
        { //for moving between chapters

            selectedChapter = to;
            
            float target = (to > currentMenu) ? -Xoffset : Xoffset;

            RectTransform levelTransform = levelMenu.GetComponent<RectTransform>();
            LeanTween.moveX(levelTransform, levelTransform.localPosition.x + target, animationTime).setEase(easeType);

            StartCoroutine(LerpBackgroundColor(palettes[currentMenu - 1].backgroundColor, palettes[to - 1].backgroundColor, animationTime));

        }

        else if (currentMenu <= -3 && to <= -3) 
        { //for moving between tutorial panels

            selectedTutorial = to;

            float target = (to > currentMenu) ? Xoffset : -Xoffset;
            
            RectTransform tutorialTransform = tutorialMenu.GetComponent<RectTransform>();
            LeanTween.moveX(tutorialTransform, tutorialTransform.localPosition.x + target, animationTime).setEase(easeType);
        }
        
        else 
        { //for moving between menus

            float target = (to > currentMenu) ? -Yoffset : Yoffset;

            if (currentMenu >= 1) //going from ANY level menu to main menu
            {
                StartCoroutine(LerpBackgroundColor(palettes[currentMenu - 1].backgroundColor, mainColor, animationTime));
                currentMenu = 1;
            }
            if (currentMenu <= -3) //going from ANY tutorial panel to main menu
            {
                currentMenu = -3;
            }

            //special case for main menu -> ANY level menu
            if (currentMenu == 0 && to >= 1)
            {
                StartCoroutine(LerpBackgroundColor(mainColor, palettes[selectedChapter - 1].backgroundColor, animationTime));
            } //special case for leaving options menu
            else if (currentMenu == -1 && to == 0)
            {
                SaveDataManager.setPlayerPrefs();
            }

            LeanTween.moveY(MenuLookup(currentMenu).GetComponent<RectTransform>(), target, animationTime).setEase(easeType);
            LeanTween.moveY(MenuLookup(to).GetComponent<RectTransform>(), 0, animationTime).setEase(easeType);

        }

        if (to == 1) 
        { //changes menu to chapter if moving to level menu
            currentMenu = selectedChapter;
        }
        else if (to == -3)
        { //changes current menu to actual menu
            currentMenu = selectedTutorial;
        }
        else
        {
            currentMenu = to;
        }

        return;
    }

    IEnumerator TurnOffAboutBG()
    {
        yield return new WaitForSeconds(.6f);
        creditsBg.GetComponent<Image>().enabled = false;

    }

    IEnumerator LerpBackgroundColor(Color toColor, Color fromColor, float duration) 
    {
        float start = Time.time;
        while (start + duration > Time.time) 
        {
            backgroundCam.backgroundColor = Color.Lerp(toColor, fromColor, (Time.time - start) / duration);
            yield return new WaitForSeconds(0.01f);
        }
        backgroundCam.backgroundColor = fromColor;
    }

    
}
