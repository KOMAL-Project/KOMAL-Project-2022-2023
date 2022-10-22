using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//[ExecuteInEditMode]
public class ManageGame : MonoBehaviour
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

    

    [SerializeField] private bool alternatingFloorTiles;

    GameObject winSwitchInstance;

    public int width, length, levelID, chapterID;
    public Texture2D levelImage, filtersImage;
    // FloorData holds floor tile GameObjects.
    // levelData holds data for whether each tile is obstructed.
    // Unobstructed tiles are a null value.
    // Obstructed tiles are references to the gameObject occupying the tile.
    public GameObject[,] levelData, floorData;
    public int[] playerStart;
    public static int furthestLevel = 255; //change this to skip levels
    public static int furthestChapter = 255;
    public static bool levelFinishing = false;

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
    List<GameObject> wallTiles, toggleSwitchesInLevel;
    public Dictionary<string, GameObject> wallDirections;
    

    void Awake()
    {
        Application.targetFrameRate = 60;

        width = levelImage.width;
        length = levelImage.height;
        levelData = new GameObject[width, length];
        floorData = new GameObject[width, length];

        string path = SceneManager.GetActiveScene().path;
        levelID = int.Parse(path.Substring(path.IndexOf("Level ") + 6, path.IndexOf(".unity") - path.IndexOf("Level ") - 6));
        chapterID = int.Parse(path.Substring(path.IndexOf("Chapter ") + 8, path.IndexOf("/Level") - path.IndexOf("Chapter ") - 8));

        /// Set up basic floor plan (movable and empty spaces)
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < length; j++)
            {
                if (!alternatingFloorTiles) { 
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

        die = GameObject.FindGameObjectWithTag("Player");

        levelFinishing = false;

        //wallDirections = new Dictionary<string, GameObject>
        //{
        //    // left, right, up, down
        //    { "0110", wallTiles[0] }, // corner_bottom_left
        //    { "1010", wallTiles[1] }, // corner_bottom_right
        //    { "0101", wallTiles[2] }, // corner_top_left
        //    { "1001", wallTiles[3] }, // corner_top_right
        //    { "0010", wallTiles[4] }, // end_bottom
        //    { "0100", wallTiles[5] }, // end_left
        //    { "1000", wallTiles[6] }, // end_right
        //    { "0001", wallTiles[7] }, // end_top
        //    { "1110", wallTiles[8] }, // t_bottom
        //    { "0111", wallTiles[9] }, // t_left
        //    { "1011", wallTiles[10] }, // t_right
        //    { "1101", wallTiles[11] }, // t_up
        //    { "0011", wallTiles[12] }, // u-d straight
        //    { "1100", wallTiles[13] }, // l-r straight
        //    { "0000", new GameObject() }
        //};

        ReadLevel();
        //Debug.Log("norm" + levelData);
    }


    public void CheckMechanics()
    {
        foreach(GameObject t in toggleSwitchesInLevel)
        {
            ToggleSwitchController toggle = t.GetComponentInChildren<ToggleSwitchController>();
            toggle.CheckForActivation();
        }
    }






    /// <summary>
    /// Scan through the level image and instantiate the necessary objects based on the color of each pixel.
    /// </summary>
    public void ReadLevel()
    {
        bool[,] tempWallData = new bool[width, length];

        // Define lists in which we will reference generated objects
        
        // Lego Lists
        List<GameObject>[] pipSwitches = new List<GameObject>[6];
        for (int i = 0; i < pipSwitches.Length; i++) pipSwitches[i] = new List<GameObject>();
        List<GameObject>[] pipWalls = new List<GameObject>[6];
        for (int i = 0; i < pipWalls.Length; i++) pipWalls[i] = new List<GameObject>();
        List<Vector2Int>[] pipWallsPositions = new List<Vector2Int>[6];
        for (int i = 0; i < pipWallsPositions.Length; i++) pipWallsPositions[i] = new List<Vector2Int>();

        // Charge Lists
        List<GameObject>[] chargeSwitches = new List<GameObject>[4];
        for (int i = 0; i < chargeSwitches.Length; i++) chargeSwitches[i] = new List<GameObject>();
        List<GameObject>[] chargeDoors = new List<GameObject>[4];
        for (int i = 0; i < chargeDoors.Length; i++) chargeDoors[i] = new List<GameObject>();
        List<Vector2Int>[] chargeWallPositions = new List<Vector2Int>[4];
        for (int i = 0; i < chargeWallPositions.Length; i++) chargeWallPositions[i] = new List<Vector2Int>();

        // Toggle Block Lists
        List<GameObject> oBlocks = new List<GameObject>();
        List<Vector2Int> oBlockPositions = new List<Vector2Int>();
        List<Vector2Int> xBlockPositions = new List<Vector2Int>();
        List<GameObject> xBlocks = new List<GameObject>();
        List<GameObject> toggleSwitches = new List<GameObject>(); 
        //Debug.Log(chargeDoors[0]);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                // Basic Walls
                if (levelImage.GetPixel(i, j) == Color.black) tempWallData[i, j] = true; //levelData[i,j] = Instantiate(floorTile, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                // Single Use Tiles
                if (levelImage.GetPixel(i,j) == singleUseColor)
                {
                    GameObject temp = Instantiate(singleUseTilePrefab, new Vector3(i - width / 2, .51f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    temp.GetComponent<SingleUseController>().position = new Vector2Int(i, j);
                    temp.GetComponent<SingleUseController>().player = die;
                    temp.GetComponent<SingleUseController>().manager = gameObject;

                }
                // Pip Switches
                for (int k = 0; k < legoSwitchColors.Length; k++)
                {
                    if (levelImage.GetPixel(i, j) == legoSwitchColors[k])
                    {
                        GameObject temp = Instantiate(pipSwitchPrefab, new Vector3(i - width / 2, 0, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        temp.GetComponent<LegoSwitchController>().thisPos = new Vector2Int(i, j);
                        temp.GetComponent<LegoSwitchController>().type = k + 1;
                        temp.GetComponent<LegoSwitchController>().pips = GetPipFilter(i, j);
                        pipSwitches[k].Add(temp);
                    }
                }
                // Legos
                for (int k = 0; k < legoBlockColors.Length; k++)
                {
                    if (levelImage.GetPixel(i, j) == legoBlockColors[k])
                    {
                        levelData[i, j] = Instantiate(pipsWallsPrefabs[k], new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        pipWalls[k].Add(levelData[i, j]);
                        pipWallsPositions[k].Add(new Vector2Int(i, j));
                    }
                }
                // Charge Givers
                for (int k = 0; k < chargeGiveColors.Length; k++)
                {
                    if (levelImage.GetPixel(i, j) == chargeGiveColors[k])
                    {
                        GameObject temp = Instantiate(chargeSwitchPrefabs[k], new Vector3(i - width / 2, .1f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        temp.GetComponent<ChargeController>().pos = new Vector2Int(i, j);
                        chargeSwitches[k].Add(temp);
                        temp.GetComponent<ChargeController>().type = k;
                        temp.GetComponentInChildren<PipFilterController>().pips = GetPipFilter(i, j);
                    }
                }
                // Cards
                for (int k = 0; k < chargeCardColors.Length; k++)
                {
                    if (levelImage.GetPixel(i, j) == chargeCardColors[k])
                    {
                        //Debug.Log(chargeWalls[k]);
                        levelData[i, j] = Instantiate(chargeWalls[k], new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                        //Debug.Log(chargeDoors[k]);
                        chargeDoors[k].Add(levelData[i, j]);
                        chargeWallPositions[k].Add(new Vector2Int(i, j));
                    }
                }
                // Toggle Switch
                if (levelImage.GetPixel(i, j) == toggleSwitchColor)
                {
                    GameObject temp = Instantiate(toggleSwitchPrefab, new Vector3(i - width / 2, .6f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    temp.GetComponentInChildren<ToggleSwitchController>().position = new Vector2Int(i, j);
                    temp.GetComponentInChildren<ToggleSwitchController>().pips = GetPipFilter(i, j);
                    toggleSwitches.Add(temp);
                }
                // "O" Toggle Block
                if (levelImage.GetPixel(i, j) == oBlockColor)
                {
                    GameObject temp = Instantiate(oBlockPrefab, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    oBlocks.Add(temp);
                    oBlockPositions.Add(new Vector2Int(i, j));
                }
                // "X" Toggle Block
                if (levelImage.GetPixel(i, j) == xBlockColor)
                {
                    GameObject temp = Instantiate(xBlockPrefab, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    xBlocks.Add(temp);
                    temp.GetComponentInChildren<Animator>().SetBool("Activated", true);
                    xBlockPositions.Add(new Vector2Int(i, j));
                }
                // Win Switch
                if (levelImage.GetPixel(i, j) == new Color(1, 1, 0)) // Color is yellow
                {
                    die.GetComponentInChildren<DieController>().winPos = new Vector2Int(i, j);
                    winSwitchInstance = Instantiate(winTile, new Vector3(i - width / 2, .5f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                }
                // Player
                if (levelImage.GetPixel(i, j) == Color.green)
                {
                    Debug.Log(i + " " + j);
                    die.GetComponentInChildren<DieController>().position = new Vector2Int(i,j);
                    
                    die.GetComponentInChildren<DieController>().gameObject.transform.position = new Vector3(i - width / 2, 1, j - length / 2);
                }
            }

            
            // Attach Legos to their switches
            for (int j = 0; j < 6; j++)
            {
                for (int k = 0; k < pipSwitches[j].Count; k++)
                {
                    pipSwitches[j][k].GetComponent<LegoSwitchController>().wallsPos = pipWallsPositions[j];
                    pipSwitches[j][k].GetComponent<LegoSwitchController>().walls = pipWalls[j];
                }
            }
            // Attach Cards to their charges
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < chargeSwitches[j].Count; k++)
                {
                    chargeSwitches[j][k].GetComponent<ChargeController>().gatePos = chargeWallPositions[j];
                    chargeSwitches[j][k].GetComponent<ChargeController>().doors = chargeDoors[j];
                }
            }
            // Attach toggle blocks to their switches (and switches to other switches)
            foreach(GameObject t in toggleSwitches)
            {
                ToggleSwitchController tsc = t.GetComponentInChildren<ToggleSwitchController>();
                tsc.oBlocks = oBlocks;
                tsc.xBlocks = xBlocks;
                tsc.switches = toggleSwitches;
                tsc.oBlockPositions = oBlockPositions;
                tsc.xBlockPositions = xBlockPositions;
            }
            toggleSwitchesInLevel = toggleSwitches;
        }

        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < length; j++)
            {
                if (tempWallData[i, j])
                {
                    levelData[i, j] = Instantiate(wallObj, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);

                    Destroy(floorData[i, j]);


                    //int up = 0;
                    //int right = 0;
                    //int down = 0;
                    //int left = 0;

                    //if (i == 0 && j == 0)
                    //{
                    //    left = 0;
                    //    down = 0;
                    //    if (tempWallData[i + 1, j])
                    //    {
                    //        right = 1;
                    //    }
                    //    if (tempWallData[i, j + 1])
                    //    {
                    //        up = 1;
                    //    }
                    //}
                    //else if (i == 0 && j == length - 1)
                    //{
                    //    left = 0;
                    //    up = 0;
                    //    if (tempWallData[i + 1, j])
                    //    {
                    //        right = 1;
                    //    }
                    //    if (tempWallData[i, j - 1])
                    //    {
                    //        down = 1;
                    //    }
                    //}
                    //else if (i == width - 1 && j == 0)
                    //{
                    //    right = 0;
                    //    down = 0;
                    //    if (tempWallData[i - 1, j])
                    //    {
                    //        left = 1;
                    //    }
                    //    if (tempWallData[i, j + 1])
                    //    {
                    //        up = 1;
                    //    }
                    //}
                    //else if (i == width - 1 && j == length - 1)
                    //{
                    //    right = 0;
                    //    up = 0;
                    //    if (tempWallData[i - 1, j])
                    //    {
                    //        left = 1;
                    //    }
                    //    if (tempWallData[i, j - 1])
                    //    {
                    //        down = 1;
                    //    }
                    //}
                    //else if (i == 0) 
                    //{
                    //    left = 0;
                    //    if (tempWallData[i + 1, j])
                    //    {
                    //        right = 1;
                    //    }
                    //    if (tempWallData[i, j + 1])
                    //    {
                    //        up = 1;
                    //    }
                    //    if (tempWallData[i, j - 1])
                    //    {
                    //        down = 1;
                    //    }
                    //}
                    //else if (i == width - 1)
                    //{
                    //    right = 0;
                    //    if (tempWallData[i - 1, j])
                    //    {
                    //        left = 1;
                    //    }
                    //    if (tempWallData[i, j + 1])
                    //    {
                    //        up = 1;
                    //    }
                    //    if (tempWallData[i, j - 1])
                    //    {
                    //        down = 1;
                    //    }
                    //}
                    //else if (j == 0)
                    //{
                    //    down = 0;
                    //    if (tempWallData[i - 1, j])
                    //    {
                    //        left = 1;
                    //    }
                    //    if (tempWallData[i + 1, j])
                    //    {
                    //        right = 1;
                    //    }
                    //    if (tempWallData[i, j + 1])
                    //    {
                    //        up = 1;
                    //    }
                    //}
                    //else if (j == length - 1)
                    //{
                    //    up = 0;
                    //    if (tempWallData[i - 1, j])
                    //    {
                    //        left = 1;
                    //    }
                    //    if (tempWallData[i + 1, j])
                    //    {
                    //        right = 1;
                    //    }
                    //    if (tempWallData[i, j - 1])
                    //    {
                    //        down = 1;
                    //    }
                    //}
                    //else
                    //{
                    //    if (tempWallData[i - 1, j])
                    //    {
                    //        left = 1;
                    //    }
                    //    if (tempWallData[i, j - 1])
                    //    {
                    //        down = 1;
                    //    }
                    //    if (tempWallData[i + 1, j])
                    //    {
                    //        right = 1;
                    //    }
                    //    if (tempWallData[i, j + 1])
                    //    {
                    //        up = 1;
                    //    }
                    //}
                    //// Debug.Log("added tile: " + left + right + up + down);

                }
            }
        }

        //Debug.Log("ReadLevel " + levelData);
        //die.GetComponent<DieController>().position = new int[] { 4, 4 };
        //die.transform.position = new Vector3(4 - width / 2, 1, 4 - length / 2);
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

    public void LevelComplete()
    {
        levelFinishing = true;
        winSwitchInstance.GetComponentInChildren<Animator>().SetTrigger("Go");
        StartCoroutine(NextLevel());

    }

    IEnumerator NextLevel()
    {

        yield return new WaitForSecondsRealtime(5);
        if (levelID != 12) {
            furthestLevel = (furthestChapter == chapterID) ? levelID + 1 : furthestLevel;
            SceneManager.LoadSceneAsync("Scenes/Chapter " + chapterID + "/Level " + (levelID + 1));
        }
        else {
            furthestChapter++;
            furthestLevel = 0;
            SceneManager.LoadSceneAsync("Menu");
        }
    }


}
