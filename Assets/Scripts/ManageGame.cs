using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[ExecuteInEditMode]
public class ManageGame : MonoBehaviour
{
    public GameObject boardTile, faceSwitch, pipsWall, board, die;
    public int width, length;
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

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {

                if (level.GetPixel(i, j) == Color.black) levelData[i,j] = Instantiate(boardTile, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                if (level.GetPixel(i, j) == Color.cyan)
                {
                    GameObject temp = Instantiate(faceSwitch, new Vector3(i - width / 2, 0, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    temp.GetComponent<FaceSwitchController>().thisPos = new Vector2Int(i, j);
                    pipSwitches.Add(temp);
                }
                if(level.GetPixel(i,j) == Color.blue)
                {
                    levelData[i, j] = Instantiate(boardTile, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                    pipsWalls.Add(levelData[i, j]);
                }
                //Debug.Log(levelData[i, j]);
                if (level.GetPixel(i, j) == Color.red)
                {
                    Debug.Log(i + " " + j);
                    die.GetComponent<DieController>().position = new Vector2Int(i,j);
                    
                    die.transform.position = new Vector3(i - width / 2, 1, j - length / 2);
                }
            }

            foreach(GameObject g in pipSwitches)
            {
                foreach(GameObject w in pipsWalls)
                {
                    g.GetComponent<FaceSwitchController>().walls.Add(w);
                }
            }
        }

        

        //Debug.Log("ReadLevel " + levelData);
        //die.GetComponent<DieController>().position = new int[] { 4, 4 };
        //die.transform.position = new Vector3(4 - width / 2, 1, 4 - length / 2);
    }


    
}
