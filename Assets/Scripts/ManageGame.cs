using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEditor.Build.Player;
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

    private void Start()
    {
        TutorialPanel[] panels = Resources.LoadAll<TutorialPanel>("TutorialPanels");
        string tempLevelIDString = chapterID + "-" + levelID;
        foreach (TutorialPanel p in panels)
        {
            Debug.Log(tempLevelIDString + " " + p.levelID + " " + (p.levelID == tempLevelIDString));
            if (p.levelID == tempLevelIDString)
            {
                tutorialPanel.GetComponent<Image>().sprite = Sprite.Create(p.panelImage,
                    new Rect(0, 0, 750, 500), Vector2.zero);
                tutorialPanel.GetComponent<Animator>().Play("OnScreen");
                break;
            }
        }
    }

    private void Update()
    {
       // Debug.Log(inputManager.keys["generic-touch"]);
        if (inputManager.keys["generic-touch"])
        {
            tutorialPanel.GetComponent<Animator>().SetBool("Onscreen", false);
        }
    }

    /// <summary>
    /// Runs whenever the die moves. Makes mechanics do their activation checks.
    /// </summary>
    public void CheckMechanics()
    {
        Vector2Int diePos = dieController.position;
        Mechanic mec = mechanicData[diePos.x, diePos.y];
        if (mec) mec.CheckForActivation();


        foreach(List<ChargeController> l in chargeControllers)
        {
            foreach (ChargeController c in l) c.UpdateChargeStatus();
        }
    }


    public static string IDsToString(int IDLevel, int IDChapter) {
        return ("" + IDLevel + "-" + IDChapter);
    }

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
        yield return new WaitForSecondsRealtime(5);
        //Debug.Log(levelIDString + " " + levelIDString.Equals("12"));
        if (!levelIDString.Equals("12") && !levelIDString.Equals("b4")) {
            furthestLevel = (furthestChapter == chapterID) ? levelID + 1 : furthestLevel;
            Debug.Log("LEVLEIDSTERINHG " + levelIDString);
            int newLevelID = levelIDString[0] == 'b' ? int.Parse(levelIDString[1].ToString()) + 1 : levelID + 1;
            
            string newLevelIDString = levelIDString[0] == 'b' ? "b" + newLevelID : newLevelID.ToString();
            Debug.Log(chapterID + " " + levelIDString + " " + newLevelID + " " + newLevelIDString);
            SceneManager.LoadSceneAsync("Scenes/Chapter " + chapterID + "/Level " + newLevelIDString);
        }
        else {
            furthestChapter++;
            //furthestLevel = 0;
            SceneManager.LoadSceneAsync("Menu");
        }
    }
}
