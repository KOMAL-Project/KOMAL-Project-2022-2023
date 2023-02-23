using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;

public class DieController : MonoBehaviour
{
    public GameObject cameraObj, dPadObj;
    private DirectionalButtonController dPad;
    private ManageGame gm;

    int width, length;

    public Vector3Int chargeDirection;
    public ChargeController currentCharge;
    public GameObject chargeFaceObj;
    public ActionRecorder actionRec;
    [HideInInspector] public Action lastAction;
    public Vector2Int position = new Vector2Int();
    public Vector2 winPos;

    public Texture2D[] ghostTextures = new Texture2D[6];
    public Sprite[] ghosts = new Sprite[6];


    public bool canControl = true;
    private bool isMoving;
    private Animator anim;
    
    [SerializeField] List<Material> chargeFaceMaterials;
    [SerializeField] List<Mesh> chargeFaceMeshes;
    
    private float rollSpeed = 4.5f;
    
    public Dictionary<Vector3Int, int> sides = new Dictionary<Vector3Int, int>();
    [SerializeField] private AudioClip diceHit;
    private AudioSource source;
    private CameraScript cs;
    public DieOverlayController doc;
    public static int totalDiceMoves = 0;
    int chargeType;
    private static DieController instance;
    //used to get the one instance of die controller instead of using tags.
    //be careful using, will not work if used before dieController awakes
    public static DieController Instance { 
        get {return instance;} 
    }
    void Awake()
    {
        instance = this;    
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        cs = cameraObj.GetComponent<CameraScript>();
        dPadObj = GameObject.FindGameObjectWithTag("D-Pad");
        dPad = dPadObj.GetComponent<DirectionalButtonController>();

        source = MusicManager.Instance.GetComponents<AudioSource>()[1];

        // Set up sides
        sides.Add(Vector3Int.up, 1);
        sides.Add(Vector3Int.down, 6);
        sides.Add(Vector3Int.left, 2);
        sides.Add(Vector3Int.right, 5);
        sides.Add(Vector3Int.back, 3);
        sides.Add(Vector3Int.forward, 4);

        gm = ManageGame.Instance;
        width = gm.width;
        length = gm.length;

        // Die Overlay Stuff
        doc = GameObject.FindGameObjectWithTag("DieOverlay").GetComponent<DieOverlayController>();
        anim = GetComponentInChildren<Animator>();

        chargeType = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(canControl);
        if (canControl && !isMoving && Time.time >= cs.GetTimeDiff()+.1f)
        {
            HandleMovement();
            if (dPad.keys["undo"]) actionRec.Undo();
        }
        //Debug.Log(gm.levelData);


    }
    /// <summary>
    /// Returns a dictionary of the die's sides when rotated counterclockwise by 90 degrees
    /// </summary>
    /// <param name="tempSides"></param>
    /// <returns></returns>
    Dictionary<Vector3Int, int> GetClockwiseRotatedSides(Dictionary<Vector3Int, int> tempSides)
    {
        return new Dictionary<Vector3Int, int>
        {
            { Vector3Int.forward, tempSides[Vector3Int.left] },
            { Vector3Int.right, tempSides[Vector3Int.forward] },
            { Vector3Int.back, tempSides[Vector3Int.right] },
            { Vector3Int.left, tempSides[Vector3Int.back] },
            // we're not actually changing these two
            { Vector3Int.up, tempSides[Vector3Int.up] },
            { Vector3Int.down, tempSides[Vector3Int.down] }
        };
    }


    /// <summary>
    /// Gets the 3 die faces that can be seen from the player's POV
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector3Int, int> GetVisibleFaces()
    {
        Dictionary<Vector3Int, int> tempSides = new Dictionary<Vector3Int, int>(sides);

        for (int i = 0; i < cs.side + 2; i++) tempSides = GetClockwiseRotatedSides(tempSides);

        return new Dictionary<Vector3Int, int>
        {
            { Vector3Int.up, tempSides[Vector3Int.up]},
            { Vector3Int.forward, tempSides[Vector3Int.forward]},
            { Vector3Int.right, tempSides[Vector3Int.right]}
        };
    }

    /// <summary>
    /// Returns the pip count for each side face on the die. Starts with the farthest face and goes clockwise.
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector3Int, int> GetSideFaces()
    {
        // get the state of the die
        int cameraSide = cs.side;
        Dictionary<Vector3Int, int> tempSides = new Dictionary<Vector3Int, int>(sides);
        // we need to make it so that we are getting the sides in the right order, that relative to the camera.
        for (int i = 0; i < cameraSide; i++) tempSides = GetClockwiseRotatedSides(tempSides);
        return new Dictionary<Vector3Int, int>
        {
            { Vector3Int.forward, tempSides[Vector3Int.forward]},
            { Vector3Int.right, tempSides[Vector3Int.right]},
            { Vector3Int.back, tempSides[Vector3Int.back]},
            { Vector3Int.left, tempSides[Vector3Int.left]}
        };
    }

    /// <summary>
    /// Gets the 3 die faces that cannot be seen from the player's POV
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector3Int, int> GetInvisibleFaces()
    {
        int cameraSide = cs.side;
        Dictionary<Vector3Int, int> tempSides = new Dictionary<Vector3Int, int>(sides);

        for (int i = 0; i < cameraSide + 2; i++) tempSides = GetClockwiseRotatedSides(tempSides);

        return new Dictionary<Vector3Int, int>
        {
            { Vector3Int.up, tempSides[Vector3Int.down]},
            { Vector3Int.forward, tempSides[Vector3Int.back]},
            { Vector3Int.right, tempSides[Vector3Int.left]}
        };
    }


    /// <summary>
    /// Moves die faces when die goes in the negative z direction
    /// </summary>
    public void MoveBack()
    {
        Dictionary<Vector3Int, int> newSides = new Dictionary<Vector3Int, int>(sides)
        {
            [Vector3Int.up] = sides[Vector3Int.forward],
            [Vector3Int.back] = sides[Vector3Int.up],
            [Vector3Int.down] = sides[Vector3Int.back],
            [Vector3Int.forward] = sides[Vector3Int.down],
            [Vector3Int.left] = sides[Vector3Int.left],
            [Vector3Int.right] = sides[Vector3Int.right]
        };

        sides = newSides;

        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.forward) chargeDirection = Vector3Int.up;
            else if (chargeDirection == Vector3.up) chargeDirection = Vector3Int.back;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3Int.forward;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.back) chargeDirection = Vector3Int.down;
        }
    }

    /// <summary>
    /// Moves die faces when die goes in the positive z direction
    /// </summary>
    public void MoveForward()
    {
        Dictionary<Vector3Int, int> newSides = new Dictionary<Vector3Int, int>(sides)
        {
            [Vector3Int.up] = sides[Vector3Int.back],
            [Vector3Int.back] = sides[Vector3Int.down],
            [Vector3Int.down] = sides[Vector3Int.forward],
            [Vector3Int.forward] = sides[Vector3Int.up],
            [Vector3Int.left] = sides[Vector3Int.left],
            [Vector3Int.right] = sides[Vector3Int.right]
        };

        //Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;

        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.up) chargeDirection = Vector3Int.forward;
            else if (chargeDirection == Vector3.back) chargeDirection = Vector3Int.up;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3Int.back;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.forward) chargeDirection = Vector3Int.down;
        }
    }

    /// <summary>
    /// Moves die faces when die goes in the negative x direction
    /// </summary>
    public void MoveLeft()
    {
        Dictionary<Vector3Int, int> newSides = new Dictionary<Vector3Int, int>(sides)
        {
            [Vector3Int.up] = sides[Vector3Int.right],
            [Vector3Int.left] = sides[Vector3Int.up],
            [Vector3Int.down] = sides[Vector3Int.left],
            [Vector3Int.right] = sides[Vector3Int.down],
            [Vector3Int.forward] = sides[Vector3Int.forward],
            [Vector3Int.back] = sides[Vector3Int.back]
        };

        //Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;

        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.up) chargeDirection = Vector3Int.left;
            else if (chargeDirection == Vector3.right) chargeDirection = Vector3Int.up;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3Int.right;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.left) chargeDirection = Vector3Int.down;
        }
    }

    /// <summary>
    /// Moves die faces when die goes in the positive x direction
    /// </summary>
    public void MoveRight()
    {
        Dictionary<Vector3Int, int> newSides = new Dictionary<Vector3Int, int>(sides)
        {
            [Vector3Int.up] = sides[Vector3Int.left],
            [Vector3Int.left] = sides[Vector3Int.down],
            [Vector3Int.down] = sides[Vector3Int.right],
            [Vector3Int.right] = sides[Vector3Int.up],
            [Vector3Int.forward] = sides[Vector3Int.forward],
            [Vector3Int.back] = sides[Vector3Int.back]
        };

        //Debug.Log(sides[Vector3.up] + " => " + newSides[Vector3.up]);
        sides = newSides;

        if (chargeDirection != Vector3.zero)
        {
            if (chargeDirection == Vector3.up) chargeDirection = Vector3Int.right;
            else if (chargeDirection == Vector3.left) chargeDirection = Vector3Int.up;
            else if (chargeDirection == Vector3.down) chargeDirection = Vector3Int.left;
            //charge side faces down, resets
            else if (chargeDirection == Vector3.right) chargeDirection = Vector3Int.down;
        }
    }

    /// <summary>
    /// We use an array of input characters to map an input to a direction.
    /// The order of the elements are offset by an amount based on the camera angle.
    /// This makes it so that the die always moves relative to the camera (left arrow always makes die go left)
    /// </summary>
    void HandleMovement()
    {
        int x = position.x;
        int y = position.y;

        string[] keys = new string[] { "w", "a", "s", "d" };

        Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.left, Vector3.back, Vector3.right };
        Action[] moves = new Action[] { MoveForward, MoveLeft, MoveBack, MoveRight };


        int index = InputToIndex();
        if (index < 0 || isMoving) return; // check if no button is pressed or if the die is already moving
        //Debug.Log("Index: " + index);
        index = (index + cs.side) % 4;
        int newX = (int)directions[index].x + x;
        int newY = (int)directions[index].y + y;
        if (newX > gm.levelData.GetLength(0) || newX < 0 || newY > gm.levelData.GetLength(1) || newY < 0) return;// checking if new move is out of level
        if (gm.levelData[x + (int)directions[index].x, y + (int)directions[index].z]) return;// checking if new move spot is occupied
        
        var anchor = transform.position + directions[index] * .5f + new Vector3(0.0f, -0.5f, 0.0f); // the point around which we are rotating
        var axis = Vector3.Cross(Vector3.up, directions[index]); // axis is the vector orthagonal to the plane formed by direction and y axis

        lastAction = moves[index];

        StartCoroutine(Roll(anchor, axis, moves[index], new Vector2Int((int)directions[index].x, (int)directions[index].z)));

        // Die Overlay rolling
        var overlayParentRot = doc.anchorRotations;
        StartCoroutine(doc.RollOverlay(axis, rollSpeed, cs.side));

        //var visible = GetVisibleFaces();
        //Debug.Log(visible[Vector3Int.up] + " " + visible[Vector3Int.forward] + " " + visible[Vector3Int.right]);
    }
    /// <summary>
    /// Returns the indexes of directional buttons that have been pressed
    /// </summary>
    /// <returns></returns>
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
    /// <param name="moveFunc"></param>
    /// <param name="moveVec"></param>
    /// <returns></returns>
    IEnumerator Roll(Vector3 anchor, Vector3 axis, Action moveFunc, Vector2Int moveVec) {
        isMoving = true;

        for (int i = 0; i < (90 /rollSpeed); i++) 
        {
            transform.RotateAround(anchor, axis, rollSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        if (source is not null) {
            source.PlayOneShot(diceHit, 0.7f);
        }   
        totalDiceMoves++;
        position += moveVec;
        WinCheck();
        
        moveFunc();
        gm.CheckMechanics();

        actionRec.Record();
        isMoving = false;

    }
    /// <summary>
    /// Handles the position and rotation of the die while it is moving between spaces instantly.
    /// </summary>
    /// <param name="anchor"></param>
    /// <param name="axis"></param>
    /// <param name="func"></param>
    /// <param name="moveVec"></param>
    void RollInstant(Vector3 anchor, Vector3 axis, Action func, Vector2Int moveVec) {
        isMoving = true;

        transform.RotateAround(anchor, axis, 90);
        
        if (source is not null) {
            source.PlayOneShot(diceHit, 0.7f);
        }   
        totalDiceMoves++;
        position += moveVec;
        WinCheck();
        
        func();
        gm.CheckMechanics();

        actionRec.Record();
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
            anim.SetTrigger("Go");
            Debug.Log(cameraObj);
            cameraObj.GetComponentInChildren<Animator>().SetTrigger("Go");
        }
    }
    
    /// <summary>
    /// Applies Charge of type "type" to the face in a direction.
    /// </summary>
    /// <param name="type"></param>
    public void PowerUp(int type, Vector3Int direction)
    {
        String[] animStrings = { "Power Up Blue", "Power Up Red", "Power Up Yellow", "Power Up Green" };

        chargeFaceObj.transform.position = this.gameObject.transform.position + Vector3.Scale(new Vector3(0.6f, 0.6f, 0.6f), direction);
        chargeFaceObj.transform.eulerAngles = Vector3.Scale(new Vector3(90, 90, 90), direction);
        chargeFaceObj.GetComponent<MeshRenderer>().material = chargeFaceMaterials[type];
        chargeFaceObj.GetComponent<MeshFilter>().mesh = chargeFaceMeshes[type];

        doc.PowerDown();
        doc.PowerUp(chargeFaceMeshes[type], chargeFaceMaterials[type]);
        Debug.Log(type + " " + animStrings[type]);
        anim.Play(animStrings[type]);

        chargeType = type;
        Debug.Log("powering up");
        sides[Vector3Int.down] = 7 + type;
        Debug.Log(sides[Vector3Int.down]);
        //Debug.Log(type);
    }

    /// <summary>
    /// Removes any existing charges on the die.
    /// </summary>
    /// <param name="playAnim"></param>
    public void PowerDown(bool playAnim = false)
    {
        chargeFaceObj.GetComponent<MeshFilter>().mesh = null;

        Debug.Log("powered down");
        if (chargeDirection != Vector3Int.zero) sides[chargeDirection] = 7 - sides[Vector3Int.zero - chargeDirection]; // The pips on opposing sides of a die always add up to 7
                                                                                                                       // we can use this to find what a side is supposed to be
        else sides[Vector3Int.down] = 7 - sides[Vector3Int.up];
        //Debug.Log(sides[Vector3Int.down]);
        chargeType = 0;
        chargeDirection = Vector3Int.zero;
        doc.PowerDown();
        if(playAnim) anim.Play("Power Down");
    }

    public bool getIsMoving() { 
        return isMoving; 
    }
}
