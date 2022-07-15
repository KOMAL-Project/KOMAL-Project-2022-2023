using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageGame : MonoBehaviour
{
    public GameObject BoardTile, board, die;
    public int width, length;
    public Texture2D level;
    public GameObject[,] levelData;
    public int[] playerStart;
    
    // Start is called before the first frame update
    void Start()
    {
        width = level.width;
        length = level.height;
        levelData = new GameObject[width, length];
        
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < length; j++)
            {
                Instantiate(BoardTile, new Vector3(i-width/2, 0, j-length/2), new Quaternion(0, 0, 0, 0), board.transform);
            }
        }
        ReadLevel();
        //Debug.Log("norm" + levelData);
    }

    public void ReadLevel()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (level.GetPixel(i, j) == Color.black) levelData[i,j] = Instantiate(BoardTile, new Vector3(i - width / 2, 1, j - length / 2), new Quaternion(0, 0, 0, 0), board.transform);
                //Debug.Log(levelData[i, j]);
                if (level.GetPixel(i, j) == Color.red)
                {
                    die.GetComponent<DieController>().position = new int[] { i, j };
                    die.transform.position = new Vector3(i - width / 2, 1, j - length / 2);
                }
            }
        }

        //Debug.Log("ReadLevel " + levelData);
    }


    
}
