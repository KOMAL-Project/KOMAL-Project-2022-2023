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

    void ConnectMechanics()
    {
        // Attach Legos to their switches
        for (int j = 0; j < 6; j++)
        {
            for (int k = 0; k < legoSwitchesInLevel[j].Count; k++)
            {
                legoSwitchesInLevel[j][k].GetComponent<LegoSwitchController>().wallsPos = legoWallPositionsInLevel[j];
                legoSwitchesInLevel[j][k].GetComponent<LegoSwitchController>().walls = legoWallsInLevel[j];
            }
        }
        // Attach Cards to their charges
        for (int j = 0; j < 4; j++)
        {
            for (int k = 0; k < chargeSwitchesInLevel[j].Count; k++)
            {
                chargeSwitchesInLevel[j][k].GetComponent<ChargeController>().gatePos = chargeCardPositionsInLevel[j];
                chargeSwitchesInLevel[j][k].GetComponent<ChargeController>().doors = chargeCardsInLevel[j];
            }
        }
        // Attach toggle blocks to their switches (and switches to other switches)
        foreach (GameObject t in toggleSwitchesInLevel)
        {
            ToggleSwitchController tsc = t.GetComponentInChildren<ToggleSwitchController>();
            tsc.oBlocks = oBlocksInLevel;
            tsc.xBlocks = xBlocksInLevel;
            tsc.switches = toggleSwitchesInLevel;
            tsc.oBlockPositions = oBlockPositionsInLevel;
            tsc.xBlockPositions = xBlockPositionsInLevel;
        }
    }

    /// <summary>
    /// Scan through the level image and instantiate the necessary objects based on the color of each pixel.
    /// </summary>
    public void ReadLevel()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                // Get the color of the level Image pixel
                Color pixel = levelImage.GetPixel(i, j);
                // Basic Walls
                if (pixel == Color.black)
                {
                    levelData[i, j] = Instantiate(wallObj, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    Destroy(floorData[i, j]);
                }
                // Single Use Tiles
                if (pixel == singleUseColor)
                {
                    GameObject temp = Instantiate(singleUseTilePrefab, new Vector3(i - width / 2, .51f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);

                    SingleUseController suc = temp.GetComponent<SingleUseController>();
                    suc.position = new Vector2Int(i, j);
                    suc.player = die;
                    suc.manager = gameObject;
                    singleUseTilesInLevel.Add(temp);

                }
                
                for (int k = 0; k < legoSwitchColors.Length; k++)
                {
                    // Lego Switches
                    if (pixel == legoSwitchColors[k])
                    {
                        GameObject temp = Instantiate(pipSwitchPrefab, new Vector3(i - width / 2, 0, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        LegoSwitchController lsc = temp.GetComponent<LegoSwitchController>();
                        lsc.thisPos = new Vector2Int(i, j);
                        lsc.type = k + 1;
                        lsc.pips = GetPipFilter(i, j);
                        legoSwitchesInLevel[k].Add(temp);
                    }
                    // Legos
                    if (pixel == legoBlockColors[k])
                    {
                        levelData[i, j] = Instantiate(pipsWallsPrefabs[k], new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        legoWallsInLevel[k].Add(levelData[i, j]);
                        legoWallPositionsInLevel[k].Add(new Vector2Int(i, j));
                    }
                }
                
                for (int k = 0; k < chargeGiveColors.Length; k++)
                {
                    // Charge Givers    
                    if (pixel == chargeGiveColors[k])
                    {
                        GameObject temp = Instantiate( chargeSwitchPrefabs[k], new Vector3(i - width / 2, 0f, j - length / 2 ), new Quaternion(0, 0, 0, 0), board.transform);
                        ChargeController chc = temp.GetComponent<ChargeController>();
                        chc.pos = new Vector2Int(i, j);
                        chc.type = k;
                        chc.pips = GetPipFilter(i, j);
                        chargeSwitchesInLevel[k].Add(temp);
                    }
                    // Cards
                    if (pixel == chargeCardColors[k])
                    {
                        levelData[i, j] = Instantiate(chargeWalls[k], new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        chargeCardsInLevel[k].Add(levelData[i, j]);
                        chargeCardPositionsInLevel[k].Add(new Vector2Int(i, j));
                    }
                }
                // Toggle Switch
                if (pixel == toggleSwitchColor)
                {
                    GameObject temp = Instantiate(toggleSwitchPrefab, new Vector3(i - width / 2, .6f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    ToggleSwitchController tsc = temp.GetComponentInChildren<ToggleSwitchController>();
                    tsc.position = new Vector2Int(i, j);
                    tsc.pips = GetPipFilter(i, j);
                    toggleSwitchesInLevel.Add(temp);
                }
                // "O" Toggle Block
                if (pixel == oBlockColor)
                {
                    GameObject temp = Instantiate(oBlockPrefab, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    oBlocksInLevel.Add(temp);
                    oBlockPositionsInLevel.Add(new Vector2Int(i, j));
                }
                // "X" Toggle Block
                if (pixel == xBlockColor)
                {
                    GameObject temp = Instantiate(xBlockPrefab, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    xBlocksInLevel.Add(temp);
                    temp.GetComponentInChildren<Animator>().SetBool("Activated", true);
                    xBlockPositionsInLevel.Add(new Vector2Int(i, j));
                }
                // Win Switch
                if (pixel == new Color(1, 1, 0)) // Color is yellow
                {
                    die.GetComponentInChildren<DieController>().winPos = new Vector2Int(i, j);
                    winSwitchInstance = Instantiate(winTile, new Vector3(i - width / 2, .5f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                }
                // Player
                if (pixel == Color.green)
                {
                    //Debug.Log(i + " " + j);
                    die.GetComponentInChildren<DieController>().position = new Vector2Int(i,j);
                    
                    die.GetComponentInChildren<DieController>().gameObject.transform.position = new Vector3(i - width / 2, 1, j - length / 2);
                }
            }
        }
    }

    /// <summary>
    /// Looks at pixel i,j on Texture2D "filters",
    /// and returns 0-6 based on the pixel color. 
    /// a 0 is returned if the color of the object is not one of the 6 colors
    /// defined in pipFilterColors.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public int GetPipFilter(int i, int j)
    {
        for (int p = 0; p < 6; p++)
            if (filtersImage.GetPixel(i, j) == pipFilterColors[p]) return p + 1;
        return 0;
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
            
            string newLevelIDString = levelIDString == "-1" ? "b" + newLevelID : newLevelID.ToString();
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
