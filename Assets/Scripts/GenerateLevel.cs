using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GenerateLevel : MonoBehaviour
{
    // Prefabs for mechanics
    public GameObject floorTile,
        pipSwitchPrefab, winTile, board, die, singleUseTilePrefab, wallObj,
        toggleSwitchPrefab, oBlockPrefab, xBlockPrefab;

    // Prefab variant things
    public GameObject[] floorTiles = new GameObject[4];
    public GameObject[] pipsWallsPrefabs = new GameObject[6];
    public GameObject[] chargeWalls = new GameObject[4];
    public GameObject[] chargeSwitchPrefabs = new GameObject[4];

    TMP_Text levelNumberText;

    [SerializeField] private bool alternatingFloorTiles;

    public GameObject winSwitchInstance;

    public int width, length, levelID, chapterID;
    string levelIDString = "-1";
    public Texture2D levelImage, filtersImage;
    // FloorData holds floor tile GameObjects.
    // levelData holds data for whether each tile is obstructed.
    // Unobstructed tiles are a null value.
    // Obstructed tiles are references to the gameObject occupying the tile.
    public GameObject[,] levelData, floorData;
    public int[] playerStart;

       
    Color singleUseColor = new Color32(128, 128, 128, 255);

    // Toggle Switch and Block Colors
    Color toggleSwitchColor = new Color32(255, 207, 158, 255);
    Color xBlockColor = new Color32(248, 114, 36, 255);
    Color oBlockColor = new Color32(13, 21, 218, 255);

    Color[] pipFilterColors = new Color[]
    {
        new Color32(250, 72, 81, 255), // 1 Pip
        new Color32(250, 135, 72, 255), // 2 Pips
        new Color32(219, 237, 24, 255), // 3 Pips
        new Color32(72, 250, 108, 255), // 4 Pips
        new Color32(72, 178, 250, 255), // 5 Pips
        new Color32(185, 72, 250, 255)  // 6 Pips
    };

    Color[] legoSwitchColors = new Color[]
    {
        new Color32(255, 100, 255, 255), // Red
        new Color32(230, 100, 255, 255), // Green
        new Color32(205, 100, 255, 255), // Blue
        new Color32(180, 100, 255, 255), // Yellow
        new Color32(155, 100, 255, 255), // Purple
        new Color32(130, 100, 255, 255)  // Black
    };
    Color[] legoBlockColors = new Color[]
    {
        new Color32(255, 0, 255, 255), // Red
        new Color32(230, 0, 255, 255), // Green
        new Color32(205, 0, 255, 255), // Blue
        new Color32(180, 0, 255, 255), // Yellow
        new Color32(155, 0, 255, 255), // Purple
        new Color32(130, 0, 255, 255)  // Black
    };
    Color[] chargeGiveColors = new Color[]
    {
        new Color32(255, 77,  77, 255), // Blue Triangle
        new Color32(255, 107, 77, 255), // Red Square
        new Color32(255, 137, 77, 255), // Yellow Cross
        new Color32(255, 167, 77, 255)  // Green Circle
    };
    Color[] chargeCardColors = new Color[]
    {
        new Color32(255, 77,  0, 255), // Blue Triangle
        new Color32(255, 107, 0, 255), // Red Square
        new Color32(255, 137, 0, 255), // Yellow Cross
        new Color32(255, 167, 0, 255)  // Green Circle
    };

    // Lists of mechanics in the level at a time

    public List<GameObject> wallTiles, toggleSwitchesInLevel, singleUseTilesInLevel,
        xBlocksInLevel, oBlocksInLevel;

    public List<GameObject>[]
        chargeSwitchesInLevel, chargeCardsInLevel,
        legoSwitchesInLevel, legoWallsInLevel;
    public List<Vector2Int>[] legoWallPositionsInLevel, chargeCardPositionsInLevel;
    public List<Vector2Int> xBlockPositionsInLevel, oBlockPositionsInLevel;
    public Dictionary<string, GameObject> wallDirections;

    ManageGame mg;
    void UpdateManageGame()
    {
        Debug.Log(levelData + " " + floorData);
        mg.levelData = levelData;
        mg.floorData = floorData;
        mg.playerStart = playerStart;
        mg.length = length;
        mg.width = width;
        mg.levelID = levelID;

        mg.wallTiles = wallTiles;
        mg.toggleSwitchesInLevel = toggleSwitchesInLevel;
        mg.singleUseTilesInLevel = singleUseTilesInLevel;
        mg.xBlocksInLevel = xBlocksInLevel;
        mg.oBlocksInLevel = oBlocksInLevel;

        mg.chargeCardsInLevel = chargeCardsInLevel;
        mg.chargeSwitchesInLevel = chargeSwitchesInLevel;
        mg.legoSwitchesInLevel = legoSwitchesInLevel;
        mg.legoWallsInLevel = legoWallsInLevel;

        mg.legoWallPositionsInLevel = legoWallPositionsInLevel;
        mg.chargeCardPositionsInLevel = chargeCardPositionsInLevel;
        mg.xBlockPositionsInLevel = xBlockPositionsInLevel;
        mg.oBlockPositionsInLevel = oBlockPositionsInLevel;

        mg.winSwitchInstance = winSwitchInstance;
    }

    void GenerateFloorTiles()
    {
        /// Set up basic floor plan (movable and empty spaces)
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (!alternatingFloorTiles)
                {
                    floorData[i, j] = Instantiate(floorTile, new Vector3(i - width / 2, 0, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                }
                else
                {
                    GameObject temp;

                    if (i % 2 == 0 && j % 2 == 0) temp = floorTiles[0];
                    else if (i % 2 == 1 && j % 2 == 0) temp = floorTiles[1];
                    else if (i % 2 == 0 && j % 2 == 1) temp = floorTiles[2];
                    else temp = floorTiles[3];

                    floorData[i, j] = Instantiate(temp, new Vector3(i - width / 2, 0.4f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                }
            }
        }
    }
    /// <summary>
    /// Logs and checks if the levelImage and filtersImage files have been set incorrectly.
    /// </summary>
    void CheckIfImageIsNull()
    { //method specifically to check and log if image files are set incorrectly
        if (levelImage is null)
        {
            Debug.Log("The level image has not been set correctly. Either the file is not in the right location or has not been assigned.");
            Debug.Log("Please format the level image as '(chapterID)-(levelID)' and the chapter folder as 'Chapter #'. E.G. '1-1' in 'Chapter 1'");
            levelImage = Resources.Load<Texture2D>("Template");
        }
        if (filtersImage is null)
        {
            Debug.Log("The level filter image has not been set correctly. Either the file is not in the right location or has not been assigned.");
            Debug.Log("Please format the level filter image as '(chapterID)-(levelID)p' and the chapter folder as 'Chapter #'. E.G. '1-1p' in 'Chapter 1'");
            filtersImage = Resources.Load<Texture2D>("Template-p");
        }
    }
    /// <summary>
    /// Sets levelID, LevelIDString, levelImage, and filtersImage to the correct files and IDs based on the path determined by the given path.
    /// </summary>
    /// <param name="path"></param>
    void SetLevelImages(string path)
    {
        if (path.Contains("Chapter ") && path.Contains("Level "))
        {
            try
            {
                levelIDString = path.Substring(path.IndexOf("Level ") + 6, path.IndexOf(".unity") - path.IndexOf("Level ") - 6);
                chapterID = int.Parse(path.Substring(path.IndexOf("Chapter ") + 8, path.IndexOf("/Level") - path.IndexOf("Chapter ") - 8));

                if (levelIDString.Contains("b"))
                { //different paths if bonus
                    levelID = int.Parse(levelIDString.Substring(1));
                    levelImage = Resources.Load<Texture2D>("Chapter " + chapterID + "/Level " + chapterID + "-b" + levelID);
                    filtersImage = Resources.Load<Texture2D>("Chapter " + chapterID + "/Level " + chapterID + "-b" + levelID + "p");
                }
                else
                {
                    levelID = int.Parse(levelIDString);
                    levelImage = Resources.Load<Texture2D>("Chapter " + chapterID + "/Level " + chapterID + "-" + levelID);
                    filtersImage = Resources.Load<Texture2D>("Chapter " + chapterID + "/Level " + chapterID + "-" + levelID + "p");
                }
                CheckIfImageIsNull();
            }
            catch (System.FormatException)
            {
                Debug.Log("The scene name or chapter folder name is not configured correctly! Please format the level and chapter name as 'Level #' and 'Chapter #'.");
            }
        }
        else
        {
            CheckIfImageIsNull();
        }


    }

    void Awake()
    {
        Application.targetFrameRate = 60;
        board = transform.GetChild(0).gameObject;
        string path = SceneManager.GetActiveScene().path;
        SetLevelImages(path);


        width = levelImage.width;
        length = levelImage.height;
        levelData = new GameObject[width, length];
        floorData = new GameObject[width, length];

        mg = GetComponent<ManageGame>();

        // Prepare the lists of GameObjects
        // Set up lists of GameObjects:
        legoSwitchesInLevel = new List<GameObject>[6];
        for (int i = 0; i < legoSwitchesInLevel.Length; i++) legoSwitchesInLevel[i] = new List<GameObject>();
        legoWallsInLevel = new List<GameObject>[6];
        for (int i = 0; i < legoWallsInLevel.Length; i++) legoWallsInLevel[i] = new List<GameObject>();
        legoWallPositionsInLevel = new List<Vector2Int>[6];
        for (int i = 0; i < legoWallPositionsInLevel.Length; i++) legoWallPositionsInLevel[i] = new List<Vector2Int>();

        // Charge Lists
        chargeSwitchesInLevel = new List<GameObject>[4];
        for (int i = 0; i < chargeSwitchesInLevel.Length; i++) chargeSwitchesInLevel[i] = new List<GameObject>();
        chargeCardsInLevel = new List<GameObject>[4];
        for (int i = 0; i < chargeCardsInLevel.Length; i++) chargeCardsInLevel[i] = new List<GameObject>();
        chargeCardPositionsInLevel = new List<Vector2Int>[4];
        for (int i = 0; i < chargeCardPositionsInLevel.Length; i++) chargeCardPositionsInLevel[i] = new List<Vector2Int>();

        // Toggle Block Lists
        oBlocksInLevel = new List<GameObject>();
        oBlockPositionsInLevel = new List<Vector2Int>();
        xBlockPositionsInLevel = new List<Vector2Int>();
        xBlocksInLevel = new List<GameObject>();
        toggleSwitchesInLevel = new List<GameObject>();

        //sets IDs and Level Data if scene is named correctly - if its not named to template, nothing is set.

        width = levelImage.width;
        length = levelImage.height;
        levelData = new GameObject[width, length];
        floorData = new GameObject[width, length];


        GenerateFloorTiles();

        die = GameObject.FindGameObjectWithTag("Player");

        ReadLevel();
        ConnectMechanics();
        //Debug.Log("norm" + levelData);
        SetUpLevelText();
        UpdateManageGame();

    }
    /// <summary>
    /// "Connects" mechanics by giving switches the references to other switches and walls that they need to function.
    /// </summary>
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
                        GameObject temp = Instantiate(chargeSwitchPrefabs[k], new Vector3(i - width / 2, .1f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
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
                    die.GetComponentInChildren<DieController>().position = new Vector2Int(i, j);

                    die.GetComponentInChildren<DieController>().gameObject.transform.position = new Vector3(i - width / 2, 1, j - length / 2);
                }
            }
        }
    }

    /// <summary>
    /// Changes the level number text at the top of the screen to match that of the current level
    /// </summary>
    void SetUpLevelText()
    {
        // Set Level Number
        levelNumberText = GameObject.FindGameObjectWithTag("LevelNumberText").GetComponent<TMP_Text>();
        // Bonus Level Changes
        string chapterAftertext = levelIDString[0] == 'b' ? " (Bonus)" : "";
        string levelIDStringTemp = levelIDString[0] == 'b' ? levelIDString[1].ToString() : levelIDString;
        // Put all of the text together
        string levelText = "Chapter " + chapterID + chapterAftertext + "\nLevel " + levelIDStringTemp;
        levelNumberText.text = (levelIDString == "12" || levelIDString == "b4") ? levelText + "\n(Final)" : levelText;
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
}
