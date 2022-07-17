using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DieController : MonoBehaviour
{
    public GameObject cameraObj;
    private ManageGame gm;

    int width, length;

    public Vector3 chargeDirection;
    public ChargeController currentCharge;

    public Vector2Int position = new Vector2Int();
    public Vector2 winPos;

    public Texture2D[] ghostTextures = new Texture2D[6];
    public Sprite[] ghosts = new Sprite[6];

    public GameObject frontFace, backFace, leftFace, rightFace;

    public bool canControl = true;
    private bool isMoving;

    
    [SerializeField] List<Material> spades, hearts, clubs, diamonds;
    [SerializeField] List<Material>[] mt; 
    [SerializeField] Material baseMT;
    
    private float rollSpeed = 6.0f;

    public Dictionary<Vector3, int> sides = new Dictionary<Vector3, int>();

    public static int totalDiceMoves = 0;

    [SerializeField] private AudioClip diceHit;
    private AudioSource source;


    // Start is called before the first frame update
    void Awake()
    {
        mt = new List<Material>[]{spades, hearts, clubs, diamonds };
        // set up ghosts
        for (int i = 0; i < 6; i++)
        {
            Rect rect = new Rect(0, 0, 10, 10);
            ghosts[i] = Sprite.Create(ghostTextures[i], rect, new Vector2(.5f, .5f));
        }

        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        Debug.Log(cameraObj);

        source = GameObject.FindGameObjectWithTag("Audio").GetComponents<AudioSource>()[1];

        // Set up sides
        sides.Add(Vector3.up, 1);
        sides.Add(Vector3.down, 6);
        sides.Add(Vector3.left, 2);
        sides.Add(Vector3.right, 5);
        sides.Add(Vector3.back, 3);
        sides.Add(Vector3.forward, 4);


        gm = FindObjectOfType<ManageGame>();
        width = gm.width;
        length = gm.length;
    }




    // Update is called once per frame
    void Update()
    {
        if(canControl && !isMoving) GetInput();
        //Debug.Log(gm.levelData);
        updateFaces();
        
    }

    void updateFaces()
    {
        frontFace.GetComponent<SpriteRenderer>().sprite = ghosts[sides[Vector3.forward] - 1];
        frontFace.transform.position = new Vector3(transform.position.x, .6f, transform.position.z + 1.05f);
        backFace.GetComponent<SpriteRenderer>().sprite = ghosts[sides[Vector3.back] - 1];
        backFace.transform.position = new Vector3(transform.position.x, .6f, transform.position.z - 1.05f);
        rightFace.GetComponent<SpriteRenderer>().sprite = ghosts[sides[Vector3.right] - 1];
        rightFace.transform.position = new Vector3(transform.position.x + 1.05f, .6f, transform.position.z);
        leftFace.GetComponent<SpriteRenderer>().sprite = ghosts[sides[Vector3.left] - 1];
        leftFace.transform.position = new Vector3(transform.position.x - 1.05f, .6f, transform.position.z);
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


        if (chargeDirection != Vector3.zero) {
            if (chargeDirection == Vector3.forward) chargeDirection = Vector3.up;
            else if (chargeDirection == Vector3.up) chargeDirection = Vector3.back;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3.forward;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.back) chargeDirection = Vector3.zero;
        }

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


        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.back) chargeDirection = Vector3.up;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3.back;
            else if (chargeDirection == Vector3.up) chargeDirection = Vector3.forward;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.forward) chargeDirection = Vector3.zero;
        }


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


        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.right) chargeDirection = Vector3.up;
            else if (chargeDirection == Vector3.up) chargeDirection = Vector3.left;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3.right;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.left) chargeDirection = Vector3.zero;
        }


        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;

    }

    void GetInput()
    {
        int x = position.x;
        int y = position.y;

        string[] keys = new string[] { "s", "d", "w", "a" };

        CameraScript cs = cameraObj.GetComponent<CameraScript>();

        //Debug.Log(1 + cs.side);

        


        if (Input.GetKey(keys[(1 + cs.side) % 4]) && !gm.levelData[x - 1, y] && !isMoving)
        {
            var anchor = transform.position + new Vector3(-0.5f, -0.5f, 0.0f);
            var axis = Vector3.Cross(Vector3.up, Vector3.left);

            StartCoroutine(Roll(anchor, axis, MoveLeft, new Vector2Int(-1, 0)));
        }

        if (Input.GetKey(keys[(3 + cs.side) % 4]) && !gm.levelData[x + 1, y] && !isMoving)
        {
            var anchor = transform.position + new Vector3(0.5f, -0.5f, 0.0f);
            var axis = Vector3.Cross(Vector3.up, Vector3.right);

            StartCoroutine(Roll(anchor, axis, MoveRight, new Vector2Int(1, 0)));
        }

        if (Input.GetKey(keys[(0 + cs.side) % 4]) && !gm.levelData[x, y + 1] && !isMoving)
        {
            var anchor = transform.position + new Vector3(0.0f, -0.5f, 0.5f);
            var axis = Vector3.Cross(Vector3.up, Vector3.forward);

            StartCoroutine(Roll(anchor, axis, MoveForward, new Vector2Int(0, 1)));
        }
        if (Input.GetKey(keys[(2 + cs.side) % 4]) && !gm.levelData[x, y - 1] && !isMoving)
        {
            var anchor = transform.position + new Vector3(0.0f, -0.5f, -0.5f);
            var axis = Vector3.Cross(Vector3.up, Vector3.back);

            StartCoroutine(Roll(anchor, axis, MoveBack, new Vector2Int(0, -1)));
        }

        //Debug.Log(position);
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


        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.left) chargeDirection = Vector3.up;
            else if (chargeDirection == Vector3.up) chargeDirection = Vector3.right;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3.left;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.right) chargeDirection = Vector3.zero;
        }

        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;
    }

    IEnumerator Roll(Vector3 anchor, Vector3 axis, Action func, Vector2Int moveVec) {
        isMoving = true;

        for (int i = 0; i < (90 / rollSpeed); i++) 
        {
            transform.RotateAround(anchor, axis, rollSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        source.clip = diceHit;
        source.Play();

        totalDiceMoves++;

        position += moveVec;
        WinCheck();
        frontFace.transform.position = transform.position = new Vector3(position.x - width / 2, 1, position.y - length / 2);

        func();

        isMoving = false;
    }

    void WinCheck()
    {
        if(position == winPos)
        {
            canControl = false;
            gm.LevelComplete();
            transform.rotation = new Quaternion(0, 0, 0, 0);
            GetComponentInChildren<Animator>().SetTrigger("Go");
            cameraObj.GetComponent<Animator>().SetTrigger("Go");
        }
    }
    
    public void PowerUp(int type)
    {
        GetComponentInChildren<MeshRenderer>().material = mt[type][sides[Vector3.down] - 1];
    }

    public void PowerDown()
    {
        GetComponentInChildren<MeshRenderer>().material = baseMT;
    }

}
