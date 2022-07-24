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

    private CameraScript cs;


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

    void GetInput()
    {
        int x = position.x;
        int y = position.y;

        string[] keys = new string[]   { "w", "a", "s", "d" };
        Vector3[] directions = new Vector3[] { Vector3.back, Vector3.right, Vector3.forward, Vector3.left };
        Action[] moves = new Action[] { MoveBack, MoveRight, MoveForward, MoveLeft };
        
        for(int i = 0; i < 4; i++)
        {
            int index = (i + 2+ cs.side) % 4;
            if (Input.GetKey(keys[index]) && !gm.levelData[x + (int)directions[index].x, y + (int)directions[index].z] && !isMoving)
            {

                Debug.Log(keys[index] + " " + directions[index]);
                var anchor = transform.position + directions[index] * .5f + new Vector3(0.0f, -0.5f, 0.0f);
                var axis = Vector3.Cross(Vector3.up, directions[index]);

                StartCoroutine(Roll(anchor, axis, moves[index], new Vector2Int((int)directions[index].x, (int)directions[index].z)));
                if (chargeDirection != Vector3.zero)
                {
                    if (chargeDirection == Vector3.forward) chargeDirection = Vector3.up;
                    else if (chargeDirection == Vector3.up) chargeDirection = Vector3.back;
                    else if (chargeDirection == Vector3.down) chargeDirection = Vector3.forward;
                    //charge side faces down, resets
                    else if (chargeDirection == Vector3.back) chargeDirection = Vector3.zero;
                }
            }
        }
        
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
            cameraObj.GetComponentInParent<Animator>().SetTrigger("Go");
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
