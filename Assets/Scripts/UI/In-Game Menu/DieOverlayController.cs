using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DieOverlayController : MonoBehaviour
{
    Camera overlayCam;
    DieController die;

    SpriteRenderer isoBottomPip, isoLeftPip, isoRightPip;
    SpriteRenderer topdownTopPip, topdownRightPip, topdownBottomPip, topdownLeftPip;
    GameObject dieOverlayDie; //object holding the actual die and invisible pips
    [SerializeField] GameObject rotationAnchorX, rotationAnchorY;
    [SerializeField] GameObject isoBottomPipObj, isoLeftPipObj, isoRightPipObj; //actual invisible pips
    [SerializeField] GameObject topdownTopPipObj, topdownRightPipObj, topdownBottomPipObj, topdownLeftPipObj; //actual invisible pips
    GameObject[] isoObjs, topDownObjs;
    [SerializeField] public GameObject overlayDie, chargeFaceObj; //actual die

    [SerializeField] Texture2D[] pipIconTextures;
    Sprite[] pipIcons;

    Vector2 side;
    bool overhead = false;

    Vector3 targetAnchorRotation;
    public Vector3 anchorRotations; // stores x rotation of x anchor and y rotation of y anchor
    // Note that this is NOT the distance of the diagonal, only the horizontal and vertical displacements
    float distanceFromCorner = .6f;


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
        // Iso Pips
        isoBottomPip = isoBottomPipObj.transform.GetChild(1).GetComponent<SpriteRenderer>();
        isoLeftPip = isoLeftPipObj.transform.GetChild(1).GetComponent<SpriteRenderer>();
        isoRightPip = isoRightPipObj.transform.GetChild(1).GetComponent<SpriteRenderer>();
        isoObjs = new GameObject[] { isoBottomPipObj, isoRightPipObj, isoLeftPipObj };
        // Top-Down Pips
        topdownRightPip = isoRightPipObj.transform.GetChild(1).GetComponent<SpriteRenderer>();

        die = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        UpdateInvisibleFaces();

        //rotationAnchor = dieOverlayDie.transform.parent.gameObject;
        targetAnchorRotation = rotationAnchorX.transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (side == Vector2.right)
            dieOverlayDie.transform.position = overlayCam.ScreenToWorldPoint(new Vector3(0, overlayCam.pixelHeight, overlayCam.nearClipPlane + 2)) + new Vector3(distanceFromCorner, distanceFromCorner * -1, 0);
        else
            dieOverlayDie.transform.position = overlayCam.ScreenToWorldPoint(new Vector3(overlayCam.pixelWidth, overlayCam.pixelHeight, overlayCam.nearClipPlane + 2)) + new Vector3(distanceFromCorner * -1, distanceFromCorner * -1, 0);
        UpdateInvisibleFaces();
        HandleOverlayRotation();
    }

    void HandleOverlayRotation()
    {
        //Vector3 rotEAX = rotationAnchorX.transform.eulerAngles;
        //Vector3 rotEAY = rotationAnchorY.transform.eulerAngles;
        //var xRotLerp = Mathf.LerpAngle(rotEAX.x, targetAnchorRotation.x, Time.deltaTime * 15);
        //var yRotLerp = Mathf.LerpAngle(rotEAY.y, targetAnchorRotation.y, Time.deltaTime * 15);
        rotationAnchorX.transform.eulerAngles = new Vector3(targetAnchorRotation.x, 0, 0); // new Vector3(xRotLerp, 0, 0);
        rotationAnchorY.transform.localEulerAngles = new Vector3(0, targetAnchorRotation.y, 0); // new Vector3(0, yRotLerp, 0);

        anchorRotations = targetAnchorRotation;// new Vector3(xRotLerp, yRotLerp, 0);
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
    /// Rotates the die around the given axis with a given speed. Axis MUST be a factor of 90 for this to work (otherwise we get floating point errors).
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="rollSpeed"></param>
    /// <returns></returns>
    public IEnumerator RollOverlay(Vector3 axis, float rollSpeed)
    {
        Vector3 toRotate = Quaternion.Euler(anchorRotations.x, (180 - 45), 0) * axis;
        Debug.Log(axis.ToString() + " => " + toRotate.ToString());
        axis = new Vector3(overhead ? 1: 0, 1, 0);
        Debug.DrawRay(overlayDie.transform.position, toRotate * 5, Color.red, 10f);
        Debug.Log(axis.ToString() + " => " + toRotate.ToString());
        Vector3 anchor = overlayDie.transform.position;
        for (int i = 0; i < (90 / rollSpeed); i++)
        {
            overlayDie.transform.RotateAround(anchor, toRotate, rollSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        UpdateInvisibleFaces();


    }

    /// <summary>
    /// Updates the sprites representing die faces that can't be seen from the current camera angle
    /// </summary>
    public void UpdateInvisibleFaces() {
        Dictionary<Vector3Int, int> invisPips = die.GetInvisibleFaces();
        isoBottomPip.sprite = pipIcons[invisPips[Vector3Int.up] - 1];
        isoRightPip.sprite = pipIcons[invisPips[Vector3Int.right] - 1];
        isoLeftPip.sprite = pipIcons[invisPips[Vector3Int.forward] - 1];
    }

    /// <summary>
    /// Shows charge with given mesh and material on the overlay.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="direction"></param>
    public void PowerUp(Mesh toAdd, Material meshMaterial)
    {
        chargeFaceObj.transform.position = dieOverlayDie.transform.position + new Vector3(0, -.185f, 0);
        chargeFaceObj.transform.rotation = Quaternion.Euler(0, -135, 0) ;
        chargeFaceObj.GetComponent<MeshRenderer>().material = meshMaterial;
        chargeFaceObj.GetComponent<MeshFilter>().mesh = toAdd;
        //Debug.Log(type);
    }

    /// <summary>
    /// Removes active Charge from the die overlay.
    /// </summary>
    public void PowerDown()
    {
        chargeFaceObj.GetComponent<MeshFilter>().mesh = null;
    }

    public void TurnClockwise() { targetAnchorRotation = new Vector3(targetAnchorRotation.x, targetAnchorRotation.y + 90, targetAnchorRotation.z); }

    public void TurnCounterClockwise() { targetAnchorRotation = new Vector3(targetAnchorRotation.x, targetAnchorRotation.y - 90, targetAnchorRotation.z); }

    public void switchToOverhead() 
    {
        overhead = true;
        targetAnchorRotation = new Vector3(-45, targetAnchorRotation.y + 45, targetAnchorRotation.z); 
    }

    public void switchOutOfOverhead() 
    {
        overhead = false;
        targetAnchorRotation = new Vector3(0, targetAnchorRotation.y - 45, targetAnchorRotation.z);
    }
}
