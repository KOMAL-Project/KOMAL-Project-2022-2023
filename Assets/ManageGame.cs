using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageGame : MonoBehaviour
{
    public GameObject BoardTile, board;
    public int width, length;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < length; j++)
            {
                Instantiate(BoardTile, new Vector3(i-width/2, 0, j-length/2), new Quaternion(0, 0, 0, 0), board.transform);
            }
        }
    }

    
}
