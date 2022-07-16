using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[ExecuteInEditMode]
public class ManageGame : MonoBehaviour
{
    public GameObject floorTile, pipSwitch, pipsWall, chargeSwitch, chargeWall, board, die;
    public int width, length;
    public Texture2D level;
    public GameObject[,] levelData;
    public int[] playerStart;

    public List<GameObject> wallTiles;

    public Dictionary<string, GameObject> wallDirections;
    
    // Start is called before the first frame update
    void Awake()
    {
        width = level.width;
        length = level.height;
        levelData = new GameObject[width, length];
        
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < length; j++)
            {

                Instantiate(floorTile, new Vector3(i-width/2, 0, j-length/2), new Quaternion(0, 0, 0, 0), board.transform);

            }
        }

        wallDirections = new Dictionary<string, GameObject>
        {
            // left, right, up, down
            { "0110", wallTiles[0] }, // corner_bottom_left
            { "1010", wallTiles[1] }, // corner_bottom_right
            { "0101", wallTiles[2] }, // corner_top_left
            { "1001", wallTiles[3] }, // corner_top_right
            { "0010", wallTiles[4] }, // end_bottom
            { "0100", wallTiles[5] }, // end_left
            { "1000", wallTiles[6] }, // end_right
            { "0001", wallTiles[7] }, // end_top
            { "1110", wallTiles[8] }, // t_bottom
            { "0111", wallTiles[9] }, // t_left
            { "1011", wallTiles[10] }, // t_right
            { "1101", wallTiles[11] }, // t_up
            { "0011", wallTiles[12] }, // u-d straight
            { "1100", wallTiles[13] }, // l-r straight
            { "0000", new GameObject() }
        };

        ReadLevel();
        //Debug.Log("norm" + levelData);
    }

    public void ReadLevel()
    {
        bool[,] tempWallData = new bool[width, length];

        List<GameObject> pipSwitches = new List<GameObject>();
        List<GameObject> pipsWalls = new List<GameObject>();

        List<GameObject> chargeSwitches = new List<GameObject>();
        List<GameObject> chargeWalls = new List<GameObject>();
        List<Vector2Int> chargeWallPositions = new List<Vector2Int>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                // Basic Walls
                if (level.GetPixel(i, j) == Color.black) tempWallData[i, j] = true; //levelData[i,j] = Instantiate(floorTile, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                // Pips Switches
                if (level.GetPixel(i, j) == Color.cyan)
                {
                    GameObject temp = Instantiate(pipSwitch, new Vector3(i - width / 2, 0, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    temp.GetComponent<FaceSwitchController>().thisPos = new Vector2Int(i, j);
                    pipSwitches.Add(temp);
                }
                if(level.GetPixel(i,j) == Color.blue)
                {
                    levelData[i, j] = Instantiate(floorTile, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    pipsWalls.Add(levelData[i, j]);
                }
                // Charge Switches
                if (level.GetPixel(i, j) == Color.magenta)
                {
                    GameObject temp = Instantiate(chargeSwitch, new Vector3(i - width / 2, .1f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    temp.GetComponent<ChargeController>().pos = new Vector2Int(i, j);
                    chargeSwitches.Add(temp);
                }
                if (level.GetPixel(i, j) == Color.red)
                {
                    levelData[i, j] = Instantiate(chargeWall, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    chargeWalls.Add(levelData[i, j]);
                    chargeWallPositions.Add(new Vector2Int(i, j));
                }
                // Player
                if (level.GetPixel(i, j) == Color.green)
                {
                    Debug.Log(i + " " + j);
                    die.GetComponent<DieController>().position = new Vector2Int(i,j);
                    
                    die.transform.position = new Vector3(i - width / 2, 1, j - length / 2);
                }
            }
            

            foreach(GameObject g in pipSwitches)
            {
                g.GetComponent<FaceSwitchController>().walls = pipsWalls;
            }

            foreach (GameObject g in chargeSwitches)
            {
                g.GetComponent<ChargeController>().gatePos = chargeWallPositions[0];
                g.GetComponent<ChargeController>().doors = chargeWalls;
            }
        }

        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < length; j++)
            {
                if (tempWallData[i, j])
                {
                    int up = 0;
                    int right = 0;
                    int down = 0;
                    int left = 0;

                    if (i == 0 && j == 0)
                    {
                        left = 0;
                        down = 0;
                        if (tempWallData[i + 1, j])
                        {
                            right = 1;
                        }
                        if (tempWallData[i, j + 1])
                        {
                            up = 1;
                        }
                    }
                    else if (i == 0 && j == length - 1)
                    {
                        left = 0;
                        up = 0;
                        if (tempWallData[i + 1, j])
                        {
                            right = 1;
                        }
                        if (tempWallData[i, j - 1])
                        {
                            down = 1;
                        }
                    }
                    else if (i == width - 1 && j == 0)
                    {
                        right = 0;
                        down = 0;
                        if (tempWallData[i - 1, j])
                        {
                            left = 1;
                        }
                        if (tempWallData[i, j + 1])
                        {
                            up = 1;
                        }
                    }
                    else if (i == width - 1 && j == length - 1)
                    {
                        right = 0;
                        up = 0;
                        if (tempWallData[i - 1, j])
                        {
                            left = 1;
                        }
                        if (tempWallData[i, j - 1])
                        {
                            down = 1;
                        }
                    }
                    else if (i == 0) 
                    {
                        left = 0;
                        if (tempWallData[i + 1, j])
                        {
                            right = 1;
                        }
                        if (tempWallData[i, j + 1])
                        {
                            up = 1;
                        }
                        if (tempWallData[i, j - 1])
                        {
                            down = 1;
                        }
                    }
                    else if (i == width - 1)
                    {
                        right = 0;
                        if (tempWallData[i - 1, j])
                        {
                            left = 1;
                        }
                        if (tempWallData[i, j + 1])
                        {
                            up = 1;
                        }
                        if (tempWallData[i, j - 1])
                        {
                            down = 1;
                        }
                    }
                    else if (j == 0)
                    {
                        down = 0;
                        if (tempWallData[i - 1, j])
                        {
                            left = 1;
                        }
                        if (tempWallData[i + 1, j])
                        {
                            right = 1;
                        }
                        if (tempWallData[i, j + 1])
                        {
                            up = 1;
                        }
                    }
                    else if (j == width - 1)
                    {
                        up = 0;
                        if (tempWallData[i - 1, j])
                        {
                            left = 1;
                        }
                        if (tempWallData[i + 1, j])
                        {
                            right = 1;
                        }
                        if (tempWallData[i, j - 1])
                        {
                            down = 1;
                        }
                    }
                    else
                    {
                        if (tempWallData[i - 1, j])
                        {
                            left = 1;
                        }
                        if (tempWallData[i, j - 1])
                        {
                            down = 1;
                        }
                        if (tempWallData[i + 1, j])
                        {
                            right = 1;
                        }
                        if (tempWallData[i, j + 1])
                        {
                            up = 1;
                        }
                    }
                    Debug.Log("added tile: " + left + right + up + down);
                    levelData[i, j] = Instantiate(wallDirections[left.ToString() + right.ToString() + up.ToString() + down.ToString()], new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                }
            }
        }

        //Debug.Log("ReadLevel " + levelData);
        //die.GetComponent<DieController>().position = new int[] { 4, 4 };
        //die.transform.position = new Vector3(4 - width / 2, 1, 4 - length / 2);
    }


    
}
