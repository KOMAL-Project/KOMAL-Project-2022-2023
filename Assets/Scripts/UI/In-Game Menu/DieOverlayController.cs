using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DieOverlayController : MonoBehaviour
{
    Camera overlayCam;
    GameObject camAnchor;
    DieController die;

    GameObject dieOverlayDie; //object holding the actual die and invisible pips
    [SerializeField] GameObject rotationAnchorX, rotationAnchorY, isoParent, overheadParent;
    GameObject[] isoObjs = new GameObject[3], overheadObjs = new GameObject[4];
    Animator[] isoAnims = new Animator[3], overheadAnims = new Animator[4];
    [SerializeField] public GameObject overlayDie, chargeFaceObj; //actual die

    [SerializeField] Texture2D[] pipIconTextures;
    Sprite[] pipIcons;

    Vector2 side;
    bool overhead = false;

    Vector3 targetAnchorRotation;
    public Vector3 anchorRotations; // stores x rotation of x anchor and y rotation of y anchor
    // Note that this is NOT the distance of the diagonal, only the horizontal and vertical displacements
    float distanceFromCorner = .8f;


    // Start is called before the first frame update
    void Start()
    {
        pipIcons = new Sprite[10];
        
        for (int i = 0; i < 10; i++)
        {
            Rect newRectTemplate = new Rect(0, 0, 250, 250);
            pipIcons[i] = Sprite.Create(pipIconTextures[i], newRectTemplate, new Vector2(.5f, .5f));
        }

        dieOverlayDie = transform.GetChild(2).gameObject;
        overlayCam = GetComponentInChildren<Camera>();
        
        // defining the arrays storing each of the pip icons
        for (int i = 0; i < isoParent.transform.childCount; i++)
        {
            isoObjs[i] = isoParent.transform.GetChild(i).gameObject;
            isoAnims[i] = isoObjs[i].GetComponent<Animator>();
        }
        for (int i = 0; i < overheadParent.transform.childCount; i++)
        {
            overheadObjs[i] = overheadParent.transform.GetChild(i).gameObject;
            overheadAnims[i] = overheadObjs[i].GetComponent<Animator>();
        }

        die = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        camAnchor = Camera.main.gameObject.transform.parent.parent.gameObject;
        //Debug.Log(die.name);
        //UpdateIcons();

        targetAnchorRotation = rotationAnchorX.transform.eulerAngles;
        foreach (Animator a in overheadAnims)
        {
            a.SetBool("Active", false);
            a.Play("Inactive");
        }
        foreach (Animator a in isoAnims)
        {
            a.SetBool("Active", true);
            a.Play("Active");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Lock the position of the overlay to the top right corner of the screen.
        if (side == Vector2.right)
            dieOverlayDie.transform.position = overlayCam.ScreenToWorldPoint(new Vector3(0, overlayCam.pixelHeight, overlayCam.nearClipPlane + 2)) + new Vector3(distanceFromCorner, distanceFromCorner * -1, 0);
        else
            dieOverlayDie.transform.position = overlayCam.ScreenToWorldPoint(new Vector3(overlayCam.pixelWidth, overlayCam.pixelHeight, overlayCam.nearClipPlane + 2)) + new Vector3(distanceFromCorner * -1, distanceFromCorner * -1, 0);
        UpdateIcons();
        HandleOverlayRotation();
    }
    /// <summary>
    /// Handles lerping the die's x and y anchors to match the camera angle.
    /// </summary>
    void HandleOverlayRotation()
    {
        // Copy the die rotation
        overlayDie.transform.localEulerAngles = die.transform.eulerAngles + new Vector3(0, 180 - 45, 0);
        // the following code rotates the die so it looks like how the camera is facing the die.
        Vector3 rotEAX = rotationAnchorX.transform.localEulerAngles;
        Vector3 rotEAY = rotationAnchorY.transform.localEulerAngles;
        var xRotLerp = Mathf.LerpAngle(rotEAX.x, targetAnchorRotation.x, Time.deltaTime * 15);
        var yRotLerp = Mathf.LerpAngle(rotEAY.y, targetAnchorRotation.y, Time.deltaTime * 15);
        rotationAnchorX.transform.localEulerAngles = new Vector3(xRotLerp, 0, 0);
        rotationAnchorY.transform.localEulerAngles = new Vector3(0, yRotLerp, 0);

        anchorRotations = new Vector3(xRotLerp, yRotLerp, 0);
    }

    /// <summary>
    /// Sets the position of the die overlay on the screen. Should use with Vector2.left or Vector2.right for best results.
    /// </summary>
    /// <param name="side"></param>
    public void SetSide(Vector2 side2Set)
    {
        side = side2Set;
    }


    /// <summary>
    /// "Calculates" the quaternion needed in order to make the overlay rotate properly when in overhead perspective
    /// </summary>
    /// <param name="cSide"></param>
    /// <returns></returns>
    Quaternion GetOverheadQuaternionFromCameraSide(int cSide)
    {
        switch (cSide % 4)
        {
            case 0: return Quaternion.Euler(-45, 0, 0);
            case 1: return Quaternion.Euler(0, 90, -45);
            case 2: return Quaternion.Euler(45, 180, 0);
            case 3: return Quaternion.Euler(0, 270, 45);
            default: return Quaternion.Euler(0, 0, 0); // this should not happen
        }
    }

    /// <summary>
    /// Updates the sprites representing die faces that can't be seen from the current camera angle. Works in both Isometric and Overhead modes.
    /// </summary>
    public void UpdateIcons() {
        if(overhead)
        {
            Dictionary<Vector3Int, int> sidePips = die.GetSideFaces();
            int i = 0;
            foreach(KeyValuePair<Vector3Int, int> k in sidePips)
            {
                overheadObjs[i].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = pipIcons[k.Value - 1];
                i++;
            }
        }
        else
        {
            Dictionary<Vector3Int, int> invisPips = die.GetInvisibleFaces();
            int i = 0;
            foreach (KeyValuePair<Vector3Int, int> k in invisPips)
            {
                isoObjs[i].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = pipIcons[k.Value - 1];
                i++;
            }
        }
    }

    /// <summary>
    /// Shows charge with given mesh and material on the overlay. Called when a charge is gained.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="direction"></param>
    public void PowerUp(Mesh toAdd, Material meshMaterial)
    {
        if (overhead)
        {
            chargeFaceObj.transform.position = dieOverlayDie.transform.position + new Vector3(0, -.125f, .125f);
            chargeFaceObj.transform.rotation = Quaternion.Euler(-45, 0, 0);
        }
        else
        { 
            chargeFaceObj.transform.position = dieOverlayDie.transform.position + new Vector3(0, -.185f, 0);
            chargeFaceObj.transform.rotation = Quaternion.Euler(0, 45, 0);
        }
        chargeFaceObj.GetComponent<MeshRenderer>().material = meshMaterial;
        chargeFaceObj.GetComponent<MeshFilter>().mesh = toAdd;
        //Debug.Log(type);
    }

    /// <summary>
    /// Removes active Charge from the die overlay. Called when a charge is used or lost.
    /// </summary>
    public void PowerDown()
    {
        chargeFaceObj.GetComponent<MeshFilter>().mesh = null;
    }

    /// <summary>
    /// Turns the overlay die by 90 degrees. Called whenever the main camera rotates clockwise.
    /// </summary>
    public void TurnClockwise() { targetAnchorRotation = new Vector3(targetAnchorRotation.x, targetAnchorRotation.y + 90, targetAnchorRotation.z); }

    /// <summary>
    /// Turns overlay die by 90 degrees. Called whenever the main camera rotates counterclockwise.
    /// </summary>
    public void TurnCounterClockwise() { targetAnchorRotation = new Vector3(targetAnchorRotation.x, targetAnchorRotation.y - 90, targetAnchorRotation.z); }

    /// <summary>
    /// Switches the camera and overlay to an overhead perspective.
    /// </summary>
    public void switchToOverhead() 
    {
        overhead = true;
        targetAnchorRotation = new Vector3(-45, targetAnchorRotation.y + 45, targetAnchorRotation.z);
        foreach (GameObject p in overheadObjs) p.GetComponent<Animator>().SetBool("Active", true);
        foreach (GameObject p in isoObjs) p.GetComponent<Animator>().SetBool("Active", false);
    }
    /// <summary>
    /// Switches the camera and overlay out of the overhead mode.
    /// </summary>
    public void switchOutOfOverhead() 
    {
        overhead = false;
        targetAnchorRotation = new Vector3(0, targetAnchorRotation.y - 45, targetAnchorRotation.z);
        foreach (GameObject p in overheadObjs) p.GetComponent<Animator>().SetBool("Active", false);
        foreach (GameObject p in isoObjs) p.GetComponent<Animator>().SetBool("Active", true);
    }
}
