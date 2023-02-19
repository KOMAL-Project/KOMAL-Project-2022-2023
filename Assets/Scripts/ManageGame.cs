using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
//using UnityEditor.Build.Player;
using Unity.Collections;

//[ExecuteInEditMode]
public class ManageGame : MonoBehaviour
{
    // Prefab variant things
    public GameObject[] floorTiles = new GameObject[4];
    public GameObject[] pipsWallsPrefabs = new GameObject[6];
    public GameObject[] chargeWalls = new GameObject[4];
    public GameObject[] chargeSwitchPrefabs = new GameObject[4];

    public GameObject winSwitchInstance;

    public int width, length, levelID, chapterID;
    public string levelIDString;
    // FloorData holds floor tile GameObjects.
    // levelData holds data for whether each tile is obstructed.
    // mechanicData holds data for each mechanic.
    // Unobstructed tiles are a null value.
    // Obstructed tiles are references to the gameObject occupying the tile.
    public GameObject[,] levelData, floorData;
    public Mechanic[,] mechanicData;
    public int[] playerStart;
    public static int furthestLevel = 256; //change this to skip levels, default is 0
    public static int furthestChapter = 256; //default is 1
    public static List<string> finishedLevels = new List<string>();
    public static bool levelFinishing = false;

    // Lists of mechanics in the level at a time
    public List<GameObject> wallTiles, singleUseTilesInLevel,
        xBlocksInLevel, oBlocksInLevel;
    
    public List<GameObject>[] chargeGatesInLevel, legoGatesInLevel;
    public List<Mechanic> mechanics;
    public List<ToggleSwitchController> toggleSwitchControllers;
    public List<ChargeController>[] chargeControllers;
    public List<LegoSwitchController>[] legoSwitchControllers;

    public List<Vector2Int>[] legoGatePositionsInLevel, chargeGatePositionsInLevel;
    public List<Vector2Int> xBlockPositionsInLevel, oBlockPositionsInLevel;

    //non-game managing variables
    public DieController dieController; 
    private static ManageGame instance;
    public static ManageGame Instance { //used to get the one instance of manageGame instead of using tags
        get {return instance;}
    }
    GameObject playerUI, tutorialPanel, dPad;
    bool tutorialDismissed = false;
    DirectionalButtonController inputManager;

    private void Awake()
    {
        instance = this;
        levelFinishing = false;
        
        levelData = new GameObject[0,0];
        Debug.Log("LEVELDATA" + levelData);
        floorData = new GameObject[0,0];

        dPad = GameObject.FindGameObjectWithTag("D-Pad");
        playerUI = dPad.transform.parent.gameObject;
        tutorialPanel = playerUI.transform.GetChild(playerUI.transform.childCount - 1).gameObject;
        
        inputManager = dPad.GetComponent<DirectionalButtonController>();
    }

    /// <summary>
    /// Handles whether or not to display a tutorial panel. Runs once upon level startup.
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivateTutorialPanel()
    {
        yield return new WaitForSecondsRealtime(.25f); // delay the function until some time after the level has started
        TutorialPanel[] panels = Resources.LoadAll<TutorialPanel>("TutorialPanels");
        
        // here we check through all tutorial panels to see if one of their ID's matches this levels ID
        string tempLevelIDString = chapterID + "-" + levelID;
        TutorialPanel panel = null;
        foreach (TutorialPanel p in panels)
        {
            Debug.Log(tempLevelIDString + " " + p.levelID + " " + (p.levelID == tempLevelIDString));
            if (p.levelID == tempLevelIDString) { panel = p; break; }
        }

        if (panel == null) yield break;
        // If a panel with a matching ID has been found, set the tutorial panel's image to the image of that panel and display the tutorial panel
        dieController.canControl = false;
        tutorialPanel.GetComponent<Image>().sprite = Sprite.Create(panel.panelImage,
            new Rect(0, 0, 750, 500), Vector2.zero); // Set the panel image to the at
        tutorialPanel.GetComponent<Animator>().Play("GoOnScreen");

    }

    private void Start()
    {
        StartCoroutine(ActivateTutorialPanel());
    }

    private void Update()
    {
       // Debug.Log(inputManager.keys["generic-touch"]);
        if (inputManager.keys["generic-touch"] && !tutorialDismissed)
        {
            tutorialPanel.GetComponent<Animator>().SetBool("Onscreen", false);
            dieController.canControl = true;
            tutorialDismissed = true;
        }
    }

    /// <summary>
    /// Runs whenever the die moves. Makes mechanics do their activation checks.
    /// </summary>
    public void CheckMechanics()
    {
        /*      Vector2Int diePos = dieController.position;
        Mechanic mec = mechanicData[diePos.x, diePos.y];
        if (mec) mec.CheckForActivation(); */
        foreach (Mechanic mec in mechanics) {
            mec.CheckForActivation();
        }


        foreach(List<ChargeController> l in chargeControllers)
        {
            foreach (ChargeController c in l) c.UpdateChargeStatus();
        }
    }


    public static string IDsToString(int IDLevel, int IDChapter) {
        return ("" + IDLevel + "-" + IDChapter);
    }

    /// <summary>
    /// Runs upon level completion.
    /// </summary>
    public void LevelComplete()
    {
        levelFinishing = true;
        string levelString = IDsToString(levelID, chapterID);
        if (!finishedLevels.Contains(levelString)) {
            finishedLevels.Add(levelString);
        }
        winSwitchInstance.GetComponentInChildren<Animator>().SetTrigger("Go");
        StartCoroutine(NextLevel());
    }

    /// <summary>
    /// Identifies and loads the next level and loads the menu if there are no more levels in the section.
    /// </summary>
    /// <returns></returns>
    IEnumerator NextLevel()
    {
        // Wait a few seconds for the level ending animation to complete
        yield return new WaitForSecondsRealtime(5);
        // Run the following if we are NOT in the last level in the sequence (12th normal level or 4th bonus level)
        if (!levelIDString.Equals("12") && !levelIDString.Equals("b4")) {
            furthestLevel = (furthestChapter == chapterID) ? levelID + 1 : furthestLevel;
            // Get the integer number of the next level in the sequence
            int newLevelID = levelIDString[0] == 'b' ? int.Parse(levelIDString[1].ToString()) + 1 : levelID + 1;
            // Combine the integer number of the next level with the sequence number (chapter number + b if bonus)
            string newLevelIDString = levelIDString[0] == 'b' ? "b" + newLevelID : newLevelID.ToString();
            SceneManager.LoadSceneAsync("Scenes/Chapter " + chapterID + "/Level " + newLevelIDString);
        }
        else {
            furthestChapter++;
            SceneManager.LoadSceneAsync("Menu");
        }
    }
}
