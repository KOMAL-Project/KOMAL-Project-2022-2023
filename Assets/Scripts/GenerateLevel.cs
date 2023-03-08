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
    // mechanicData holds data for each mechanic.
    // Unobstructed tiles are a null value.
    // Obstructed tiles are references to the gameObject occupying the tile.
    public GameObject[,] levelData, floorData;
    public Mechanic[,] mechanicData;
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

    public List<GameObject> wallTiles, singleUseTilesInLevel,
        xBlocksInLevel, oBlocksInLevel;
    
    public List<GameObject>[] chargeGatesInLevel, legoGatesInLevel;
    public List<Mechanic> mechanics;
    public List<ToggleSwitchController> toggleSwitchControllers;
    public List<ChargeController>[] chargeControllers;
    public List<LegoSwitchController>[] legoSwitchControllers;

    public List<Vector2Int>[] legoGatePositionsInLevel, chargeGatePositionsInLevel;
    public List<Vector2Int> xBlockPositionsInLevel, oBlockPositionsInLevel;
    public Dictionary<string, GameObject> wallDirections;
    public DieController dieController;

    ManageGame mg;
    void UpdateManageGame()
    {
        Debug.Log(levelData + " " + floorData);
        mg.levelData = levelData;
        mg.floorData = floorData;
        mg.mechanicData = mechanicData;
        mg.playerStart = playerStart;
        mg.length = length;
        mg.width = width;
        mg.levelID = levelID;
        mg.levelIDString = levelIDString;
        mg.chapterID = chapterID;

        mg.wallTiles = wallTiles;
        mg.singleUseTilesInLevel = singleUseTilesInLevel;
        mg.xBlocksInLevel = xBlocksInLevel;
        mg.oBlocksInLevel = oBlocksInLevel;

        mg.mechanics = mechanics;
        mg.toggleSwitchControllers = toggleSwitchControllers;
        mg.chargeControllers = chargeControllers;
        mg.legoSwitchControllers = legoSwitchControllers;

        mg.legoGatePositionsInLevel = legoGatePositionsInLevel;
        mg.chargeGatePositionsInLevel = chargeGatePositionsInLevel;
        mg.legoGatesInLevel = legoGatesInLevel;
        mg.chargeGatesInLevel = chargeGatesInLevel;
        mg.xBlockPositionsInLevel = xBlockPositionsInLevel;
        mg.oBlockPositionsInLevel = oBlockPositionsInLevel;

        mg.winSwitchInstance = winSwitchInstance;
        mg.dieController = dieController;
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
        // We set the frame rate here!
        Application.targetFrameRate = 60;
        board = transform.GetChild(0).gameObject;
        string path = SceneManager.GetActiveScene().path;
        SetLevelImages(path);


        width = levelImage.width;
        length = levelImage.height;
        levelData = new GameObject[width, length];
        floorData = new GameObject[width, length];

        mg = FindObjectOfType<ManageGame>(); //can change to tags for optimization


        // Prepare the lists of GameObjects
        mechanics = new List<Mechanic>();
        // Set up lists of GameObjects:
        legoSwitchControllers = new List<LegoSwitchController>[6];
        for (int i = 0; i < legoSwitchControllers.Length; i++) legoSwitchControllers[i] = new List<LegoSwitchController>();
        legoGatesInLevel = new List<GameObject>[6];
        for (int i = 0; i < legoGatesInLevel.Length; i++) legoGatesInLevel[i] = new List<GameObject>();
        legoGatePositionsInLevel = new List<Vector2Int>[6];
        for (int i = 0; i < legoGatePositionsInLevel.Length; i++) legoGatePositionsInLevel[i] = new List<Vector2Int>();

        // Charge Lists
        chargeControllers = new List<ChargeController>[4];
        for (int i = 0; i < chargeControllers.Length; i++) chargeControllers[i] = new List<ChargeController>();
        chargeGatesInLevel = new List<GameObject>[4];
        for (int i = 0; i < chargeGatesInLevel.Length; i++) chargeGatesInLevel[i] = new List<GameObject>();
        chargeGatePositionsInLevel = new List<Vector2Int>[4];
        for (int i = 0; i < chargeGatePositionsInLevel.Length; i++) chargeGatePositionsInLevel[i] = new List<Vector2Int>();

        // Toggle Block Lists
        oBlocksInLevel = new List<GameObject>();
        xBlocksInLevel = new List<GameObject>();
        oBlockPositionsInLevel = new List<Vector2Int>();
        xBlockPositionsInLevel = new List<Vector2Int>();
        toggleSwitchControllers = new List<ToggleSwitchController>();

        //sets IDs and Level Data if scene is named correctly - if its not named to template, nothing is set.

        width = levelImage.width;
        length = levelImage.height;
        levelData = new GameObject[width, length];
        floorData = new GameObject[width, length];
        mechanicData = new Mechanic[width, length];


        GenerateFloorTiles();

        die = GameObject.FindGameObjectWithTag("Player");
        dieController = die.GetComponentInChildren<DieController>();

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
            for (int k = 0; k < legoSwitchControllers[j].Count; k++)
            {
                legoSwitchControllers[j][k].gatePos = legoGatePositionsInLevel[j];
                legoSwitchControllers[j][k].gates = legoGatesInLevel[j];
                legoSwitchControllers[j][k].controllers = legoSwitchControllers[j];
            }
        }
        // Attach Cards to their charges
        for (int j = 0; j < 4; j++)
        {
            for (int k = 0; k < chargeControllers[j].Count; k++)
            {
                chargeControllers[j][k].gatePos = chargeGatePositionsInLevel[j];
                chargeControllers[j][k].gates = chargeGatesInLevel[j];
                chargeControllers[j][k].controllers = chargeControllers;
            }
        }
        // Attach toggle blocks to their switches (and switches to other switches)
        foreach (ToggleSwitchController t in toggleSwitchControllers)
        {
            t.oBlocks = oBlocksInLevel;
            t.xBlocks = xBlocksInLevel;
            t.oBlockPositions = oBlockPositionsInLevel;
            t.xBlockPositions = xBlockPositionsInLevel;
            t.switches = toggleSwitchControllers;
        }
    }

    /// <summary>
    /// Scan through the level image and instantiate the necessary objects based on the color of each pixel.
    /// </summary>
    public void ReadLevel()
    {
        DieController dieControl = die.GetComponentInChildren<DieController>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                Mechanic mec = null;
                GameObject temp = null;
                int type = -1; //if no type is declared, type is -1
                
                // Get the color of the level Image pixel
                Color pixel = levelImage.GetPixel(i, j);

                // Basic Walls
                if (pixel == Color.black)
                {
                    temp = Instantiate(wallObj, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    levelData[i, j] = temp;
                    Destroy(floorData[i, j]);
                }
                // Single Use Tiles
                else if (pixel == singleUseColor)
                {
                    temp = Instantiate(singleUseTilePrefab, new Vector3(i - width / 2, .51f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    singleUseTilesInLevel.Add(temp);
                    mec = temp.GetComponent<SingleUseController>();
                    temp = null; //DONT add it to levelData immediately

                }

                for (int k = 0; k < legoSwitchColors.Length; k++)
                {
                    // Lego Switches
                    if (pixel == legoSwitchColors[k])
                    {
                        temp = Instantiate(pipSwitchPrefab, new Vector3(i - width / 2, 0, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        type = k + 1;
                        mec = temp.GetComponent<LegoSwitchController>();
                        legoSwitchControllers[k].Add((LegoSwitchController) mec);
                        temp = null; //DONT add it to levelData immediately
                        break;
                    }
                    // Legos
                    if (pixel == legoBlockColors[k])
                    {
                        temp = Instantiate(pipsWallsPrefabs[k], new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        levelData[i, j] = temp;
                        legoGatesInLevel[k].Add(temp);
                        legoGatePositionsInLevel[k].Add(new Vector2Int(i, j));
                        break;
                    }
                }

                for (int k = 0; k < chargeGiveColors.Length; k++) {
                    // Charge Givers    
                    if (pixel == chargeGiveColors[k])
                    {
                        temp = Instantiate(chargeSwitchPrefabs[k], new Vector3(i - width / 2, .1f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        type = k;
                        mec = temp.GetComponent<ChargeController>();
                        chargeControllers[k].Add((ChargeController) mec);
                        temp = null; //DONT add it to levelData immediately
                        break;
                    }
                    // Cards
                    if (pixel == chargeCardColors[k])
                    {
                        temp = Instantiate(chargeWalls[k], new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        levelData[i, j] = temp;
                        chargeGatesInLevel[k].Add(temp);
                        chargeGatePositionsInLevel[k].Add(new Vector2Int(i, j));
                        break;
                    }
                }
                // Toggle Switch
                if (pixel == toggleSwitchColor)
                {
                    temp = Instantiate(toggleSwitchPrefab, new Vector3(i - width / 2, .6f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    mec = temp.GetComponentInChildren<ToggleSwitchController>();
                    toggleSwitchControllers.Add((ToggleSwitchController) mec);
                    temp = null; //DONT add it to levelData immediately
                }
                // "O" Toggle Block
                else if (pixel == oBlockColor)
                {
                    temp = Instantiate(oBlockPrefab, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    oBlocksInLevel.Add(temp);
                    oBlockPositionsInLevel.Add(new Vector2Int(i, j));
                }
                // "X" Toggle Block
                else if (pixel == xBlockColor)
                {
                    temp = Instantiate(xBlockPrefab, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    xBlocksInLevel.Add(temp);
                    temp.GetComponentInChildren<Animator>().SetBool("Activated", true);
                    xBlockPositionsInLevel.Add(new Vector2Int(i, j));
                }
                // Win Switch
                else if (pixel == new Color(1, 1, 0)) // Color is yellow
                {
                    dieControl.winPos = new Vector2Int(i, j);
                    winSwitchInstance = Instantiate(winTile, new Vector3(i - width / 2, .5f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                }
                // Player
                else if (pixel == Color.green)
                {
                    //Debug.Log(i + " " + j);
                    dieControl.position = new Vector2Int(i, j);
                    dieControl.gameObject.transform.position = new Vector3(i - width / 2, 1, j - length / 2);
                }

                levelData[i,j] = temp;
                mechanicData[i,j] = mec;

                if (mec != null) {
                    //attaches each value if needed
                    mec.attachValues(
                        mg
                        ,dieControl
                        ,new Vector2Int(i, j)
                        ,GetPipFilter(i, j)
                        ,type
                    );

                    mechanics.Add(mec);
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
