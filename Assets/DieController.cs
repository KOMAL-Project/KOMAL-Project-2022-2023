using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieController : MonoBehaviour
{
    public GameObject manager;
    private ManageGame gm;
    public int[] position;
    int width, length;

    Dictionary<Vector3, int> sides = new Dictionary<Vector3, int>();


    // Start is called before the first frame update
    void Start()
    {
        // Set up sides
        sides.Add(Vector3.up, 2);
        sides.Add(Vector3.down, 5);
        sides.Add(Vector3.left, 4);
        sides.Add(Vector3.right, 3);
        sides.Add(Vector3.back, 6);
        sides.Add(Vector3.forward, 1);


        gm = manager.GetComponent<ManageGame>();
        width = gm.width;
        length = gm.length;
    }



    // Update is called once per frame
    void Update()
    {
        GetInput();
        //Debug.Log(gm.levelData);
        
    }

    void GetInput()
    {
        int x = position[0];
        int y = position[1];
        
        if (Input.GetKeyDown("a") && !gm.levelData[x -1, y])
        {
            x--;
            MoveLeft();
            transform.Rotate(0, 0, 90, Space.World);

        }
        if (Input.GetKeyDown("d") && !gm.levelData[x + 1, y])
        {
            x++;
            MoveRight();
            transform.Rotate(0, 0, -90, Space.World);
        }
        if (Input.GetKeyDown("w") && !gm.levelData[x, y + 1])
        {
            y++;
            MoveForward();
            transform.Rotate(90, 0, 0, Space.World);
        }
        if (Input.GetKeyDown("s") && !gm.levelData[x, y - 1])
        {
            y--;
            MoveBack();
            transform.Rotate(-90, 0, 0, Space.World);
        }

        position = new int[] { x, y };
        transform.position = new Vector3(x - width / 2, 1, y - length / 2);
    }

    void MoveBack()
    {
        Dictionary<Vector3, int> newSides = new Dictionary<Vector3, int>(sides);
        newSides[Vector3.up] = sides[Vector3.forward];
        newSides[Vector3.back] = sides[Vector3.up];
        newSides[Vector3.down] = sides[Vector3.back];
        newSides[Vector3.forward] = sides[Vector3.down];
        newSides[Vector3.left] = sides[Vector3.left];
        newSides[Vector3.right] = sides[Vector3.right];
        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;
        
    }

    void MoveForward()
    {
        Dictionary<Vector3, int> newSides = new Dictionary<Vector3, int>(sides);
        newSides[Vector3.up] = sides[Vector3.back];
        newSides[Vector3.back] = sides[Vector3.down];
        newSides[Vector3.down] = sides[Vector3.forward];
        newSides[Vector3.forward] = sides[Vector3.up];
        newSides[Vector3.left] = sides[Vector3.left];
        newSides[Vector3.right] = sides[Vector3.right];
        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;
    }

    void MoveLeft()
    {
        Dictionary<Vector3, int> newSides = new Dictionary<Vector3, int>(sides);
        newSides[Vector3.up] = sides[Vector3.right];
        newSides[Vector3.left] = sides[Vector3.up];
        newSides[Vector3.down] = sides[Vector3.left];
        newSides[Vector3.right] = sides[Vector3.down];
        newSides[Vector3.forward] = sides[Vector3.forward];
        newSides[Vector3.back] = sides[Vector3.back];
        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;

    }

    void MoveRight()
    {
        Dictionary<Vector3, int> newSides = new Dictionary<Vector3, int>(sides);
        newSides[Vector3.up] = sides[Vector3.left];
        newSides[Vector3.left] = sides[Vector3.down];
        newSides[Vector3.down] = sides[Vector3.right];
        newSides[Vector3.right] = sides[Vector3.up];
        newSides[Vector3.forward] = sides[Vector3.forward];
        newSides[Vector3.back] = sides[Vector3.back];
        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;
    }

}
