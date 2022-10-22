using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DieController : MonoBehaviour
{
    public GameObject cameraObj, dPadObj;
    private DirectionalButtonController dPad;
    private ManageGame gm;

    int width, length;

    public Vector3 chargeDirection;
    public ChargeController currentCharge;
    public ActionRecorder actionRec;
    [HideInInspector] public Action lastAction;
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
    
    private float rollSpeed = 4.5f;
    
    public Dictionary<Vector3, int> sides = new Dictionary<Vector3, int>();
    [SerializeField] private AudioClip diceHit;
    private AudioSource source;
    private CameraScript cs;
    public static int totalDiceMoves = 0;
    
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

        cameraObj = Camera.main.gameObject;
        cs = cameraObj.GetComponentInParent<CameraScript>();
        dPadObj = GameObject.FindGameObjectWithTag("D-Pad");
        dPad = dPadObj.GetComponent<DirectionalButtonController>();

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
        UpdateFaces();
        
    }

    /// <summary>
    /// Updates the position of the ghost faces around the die
    /// </summary>
    void UpdateFaces()
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

   
    /// <summary>
    /// Moves die faces when die goes in the negative z direction
    /// </summary>
    public void MoveBack()
    {
        Dictionary<Vector3, int> newSides = new Dictionary<Vector3, int>(sides)
        {
            [Vector3.up] = sides[Vector3.forward],
            [Vector3.back] = sides[Vector3.up],
            [Vector3.down] = sides[Vector3.back],
            [Vector3.forward] = sides[Vector3.down],
            [Vector3.left] = sides[Vector3.left],
            [Vector3.right] = sides[Vector3.right]
        };

        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;

        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.forward) chargeDirection = Vector3.up;
            else if (chargeDirection == Vector3.up) chargeDirection = Vector3.back;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3.forward;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.back) chargeDirection = Vector3.zero;
        }
        gm.CheckMechanics();
    }

    /// <summary>
    /// Moves die faces when die goes in the positive z direction
    /// </summary>
    public void MoveForward()
    {
        Dictionary<Vector3, int> newSides = new Dictionary<Vector3, int>(sides)
        {
            [Vector3.up] = sides[Vector3.back],
            [Vector3.back] = sides[Vector3.down],
            [Vector3.down] = sides[Vector3.forward],
            [Vector3.forward] = sides[Vector3.up],
            [Vector3.left] = sides[Vector3.left],
            [Vector3.right] = sides[Vector3.right]
        };

        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;

        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.up) chargeDirection = Vector3.forward;
            else if (chargeDirection == Vector3.back) chargeDirection = Vector3.up;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3.back;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.forward) chargeDirection = Vector3.zero;
        }
        gm.CheckMechanics();
    }

    /// <summary>
    /// Moves die faces when die goes in the negative x direction
    /// </summary>
    public void MoveLeft()
    {
        Dictionary<Vector3, int> newSides = new Dictionary<Vector3, int>(sides)
        {
            [Vector3.up] = sides[Vector3.right],
            [Vector3.left] = sides[Vector3.up],
            [Vector3.down] = sides[Vector3.left],
            [Vector3.right] = sides[Vector3.down],
            [Vector3.forward] = sides[Vector3.forward],
            [Vector3.back] = sides[Vector3.back]
        };

        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;

        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.up) chargeDirection = Vector3.left;
            else if (chargeDirection == Vector3.right) chargeDirection = Vector3.up;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3.right;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.left) chargeDirection = Vector3.zero;
        }
        gm.CheckMechanics();
    }

    /// <summary>
    /// Moves die faces when die goes in the positive x direction
    /// </summary>
    public void MoveRight()
    {
        Dictionary<Vector3, int> newSides = new Dictionary<Vector3, int>(sides)
        {
            [Vector3.up] = sides[Vector3.left],
            [Vector3.left] = sides[Vector3.down],
            [Vector3.down] = sides[Vector3.right],
            [Vector3.right] = sides[Vector3.up],
            [Vector3.forward] = sides[Vector3.forward],
            [Vector3.back] = sides[Vector3.back]
        };

        Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;

        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.up) chargeDirection = Vector3.right;
            else if (chargeDirection == Vector3.left) chargeDirection = Vector3.up;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3.left;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.right) chargeDirection = Vector3.zero;
        }
        gm.CheckMechanics();
    }

    /// <summary>
    /// We use an array of input characters to map an input to a direction.
    /// The order of the elements are offset by an amount based on the camera angle.
    /// This makes it so that the die always moves relative to the camera (left arrow always makes die go left)
    /// </summary>
    void GetInput()
    {
        int x = position.x;
        int y = position.y;

        string[] keys = new string[] { "w", "a", "s", "d" };

        Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.left, Vector3.back, Vector3.right };
        Action[] moves = new Action[] { MoveForward, MoveLeft, MoveBack, MoveRight };


        int index = InputToIndex();
        if (index < 0 || isMoving) return;
        Debug.Log("Index: " + index);
        index = (index + cs.side) % 4;
        //if (index < 0) index += 4;
        int newX = (int)directions[index].x + x;
        int newY = (int)directions[index].y + y;
        //if (newX > gm.levelData.GetLength(0) || newX < 0 || newY > gm.levelData.GetLength(1) || newY < 0) return;// checking if new move is out of level
        if (gm.levelData[x + (int)directions[index].x, y + (int)directions[index].z]) return;// checking if new move spot is occupied
        
        var anchor = transform.position + directions[index] * .5f + new Vector3(0.0f, -0.5f, 0.0f);
        var axis = Vector3.Cross(Vector3.up, directions[index]);

        lastAction = moves[index];
        actionRec.Record();

        StartCoroutine(Roll(anchor, axis, moves[index], new Vector2Int((int)directions[index].x, (int)directions[index].z)));
                
    }
    
    int InputToIndex()
    {
        int i = 0;
        string[] keys = new string[] { "w", "a", "s", "d" };
        bool[] btns = new bool[] { dPad.keys["up"], dPad.keys["left"], dPad.keys["down"], dPad.keys["right"] };
        foreach (string k in keys)
        {
            if (Input.GetKey(k) || btns[i]) return i;
            i++;
        }
        return -1;
    }

    /// <summary>
    /// Handles the position and rotation of the die while it is moving between spaces.
    /// </summary>
    /// <param name="anchor"></param>
    /// <param name="axis"></param>
    /// <param name="func"></param>
    /// <param name="moveVec"></param>
    /// <returns></returns>
    IEnumerator Roll(Vector3 anchor, Vector3 axis, Action func, Vector2Int moveVec) {
        isMoving = true;

        for (int i = 0; i < (90 /rollSpeed); i++) 
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

    /// <summary>
    /// Checks to see if the die is on the win panel.
    /// If so, sends die flying up and ends the level.
    /// </summary>
    void WinCheck()
    {
        if(position == winPos)
        {
            canControl = false;
            gm.LevelComplete();
            transform.rotation = new Quaternion(0, 0, 0, 0);
            GetComponentInChildren<Animator>().SetTrigger("Go");
            cameraObj.GetComponentInParent<Animator>().SetTrigger("Go");
        }
    }
    
    /// <summary>
    /// Applies Charge of type "type" to the face currently facing down.
    /// </summary>
    /// <param name="type"></param>
    public void PowerUp(int type)
    {
        GetComponentInChildren<MeshRenderer>().material = mt[type][sides[Vector3.down] - 1];
    }

    /// <summary>
    /// Removes active Charge from the die.
    /// </summary>
    public void PowerDown()
    {
        GetComponentInChildren<MeshRenderer>().material = baseMT;
    }

}
