using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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
    string levelIDString = "-1";
    // FloorData holds floor tile GameObjects.
    // levelData holds data for whether each tile is obstructed.
    // Unobstructed tiles are a null value.
    // Obstructed tiles are references to the gameObject occupying the tile.
    public GameObject[,] levelData, floorData;
    public int[] playerStart;
    public static int furthestLevel = 256; //change this to skip levels, default is 0
    public static int furthestChapter = 256; //default is 1
    public static List<string> finishedLevels = new List<string>();
    public static bool levelFinishing = false;

    // Lists of mechanics in the level at a time
    public List<GameObject> wallTiles, toggleSwitchesInLevel, singleUseTilesInLevel,
        xBlocksInLevel, oBlocksInLevel;
        
    public List<GameObject>[]
        chargeSwitchesInLevel, chargeCardsInLevel, 
        legoSwitchesInLevel, legoWallsInLevel;
    public List<Vector2Int>[] legoWallPositionsInLevel, chargeCardPositionsInLevel;
    public List<Vector2Int> xBlockPositionsInLevel, oBlockPositionsInLevel;


    private void Awake()
    {
        levelFinishing = false;
        
        levelData = new GameObject[0,0];
        Debug.Log("LEVELDATA" + levelData);
        floorData = new GameObject[0,0];
    }

    /// <summary>
    /// Runs whenever the die moves. Makes mechanics do their activation checks.
    /// </summary>
    public void CheckMechanics()
    {
        foreach(List<GameObject> l in legoSwitchesInLevel)
        {
            foreach (GameObject g in l) g.GetComponent<LegoSwitchController>().CheckForActivation();
        }
        foreach(GameObject t in toggleSwitchesInLevel)
        {
            ToggleSwitchController toggle = t.GetComponentInChildren<ToggleSwitchController>();
            toggle.CheckForActivation();
        }
        foreach(List<GameObject> l in chargeSwitchesInLevel)
        {
            foreach (GameObject g in l) g.GetComponent<ChargeController>().CheckForActivation();
            foreach (GameObject g in l) g.GetComponent<ChargeController>().UpdateChargeStatus();
        }
        foreach(GameObject s in singleUseTilesInLevel) s.GetComponent<SingleUseController>().CheckForActivation();
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
        if (levelID != 12 || levelIDString == "b4") {
            furthestLevel = (furthestChapter == chapterID) ? levelID + 1 : furthestLevel;
            int newLevelID = levelIDString[0] == 'b' ? int.Parse(levelIDString[1].ToString()) + 1 : levelID + 1;
            
            string newLevelIDString = levelIDString[0] == 'b' ? "b" + newLevelID : newLevelID.ToString();
            Debug.Log(chapterID + " " + levelIDString + " " + newLevelID + " " + newLevelIDString);
            SceneManager.LoadSceneAsync("Scenes/Chapter " + chapterID + "/Level " + newLevelIDString);
        }
        else {
            furthestChapter++;
            furthestLevel = 0;
            SceneManager.LoadSceneAsync("Menu");
        }
    }


}
