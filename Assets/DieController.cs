using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInput();
    }

    void GetInput()
    {
        if (Input.GetKeyDown("space"))
        {
            print("space key was pressed");
        }
    }
}
