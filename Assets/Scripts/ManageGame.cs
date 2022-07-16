using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


//[ExecuteInEditMode]
public class ManageGame : MonoBehaviour
{
    public GameObject boardTile, 
        pipSwitch, pipsWall,
        chargeSwitch, chargeWall, 
        winTile, board, die;

    GameObject winSwitchInstance;

    public int width, length, levelID;
    public Texture2D level;
    public GameObject[,] levelData;
    public int[] playerStart;
    




    
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

                Instantiate(boardTile, new Vector3(i-width/2, 0, j-length/2), new Quaternion(0, 0, 0, 0), board.transform);

            }
        }
        ReadLevel();
        //Debug.Log("norm" + levelData);
    }

    public void ReadLevel()
    {


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
                if (level.GetPixel(i, j) == Color.black) levelData[i,j] = Instantiate(boardTile, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                // Pips Switches
                if (level.GetPixel(i, j) == Color.cyan)
                {
                    GameObject temp = Instantiate(pipSwitch, new Vector3(i - width / 2, 0, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    temp.GetComponent<FaceSwitchController>().thisPos = new Vector2Int(i, j);
                    pipSwitches.Add(temp);
                }
                if (level.GetPixel(i,j) == Color.blue)
                {
                    levelData[i, j] = Instantiate(boardTile, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
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
                if (level.GetPixel(i, j) == new Color(1, 1, 0)) // Yellow for Win Switch
                {
                    die.GetComponent<DieController>().winPos = new Vector2Int(i, j);
                    winSwitchInstance = Instantiate(winTile, new Vector3(i - width / 2, .5f, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
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

        

        //Debug.Log("ReadLevel " + levelData);
        //die.GetComponent<DieController>().position = new int[] { 4, 4 };
        //die.transform.position = new Vector3(4 - width / 2, 1, 4 - length / 2);
    }

    public void LevelComplete()
    {
        winSwitchInstance.GetComponentInChildren<Animator>().SetTrigger("Go");
        StartCoroutine(NextLevel());
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSecondsRealtime(3);
        SceneManager.LoadSceneAsync("Level " + (levelID + 1));
    }


}
