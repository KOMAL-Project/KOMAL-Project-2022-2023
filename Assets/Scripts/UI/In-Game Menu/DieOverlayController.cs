using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOverlayController : MonoBehaviour
{
    Camera overlayCam;
    DieController die;

    SpriteRenderer hiddenBottomPip, hiddenLeftPip, hiddenRightPip;
    GameObject dieOverlayDie; //object holding the actual die and invisible pips
    [SerializeField] GameObject hiddenBotomPipObj, hiddenLeftPipObj, hiddenRightPipObj; //actual invisible pips
    [SerializeField] public GameObject overlayDie, chargeFaceObj; //actual die

    [SerializeField] Texture2D[] pipIconTextures;
    Sprite[] pipIcons;

    Vector2 side;

    // Note that this is NOT the distance of the diagonal, only the horizontal and vertical displacements
    float distanceFromCorner = .55f;


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
        hiddenBottomPip = hiddenBotomPipObj.GetComponent<SpriteRenderer>();
        hiddenLeftPip = hiddenLeftPipObj.GetComponent<SpriteRenderer>();
        hiddenRightPip = hiddenRightPipObj.GetComponent<SpriteRenderer>();
        die = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        updateInvisibleFaces();
    }

    // Update is called once per frame
    void Update()
    {
        if (side == Vector2.right)
            dieOverlayDie.transform.position = overlayCam.ScreenToWorldPoint(new Vector3(0, overlayCam.pixelHeight, overlayCam.nearClipPlane + 2)) + new Vector3(distanceFromCorner, distanceFromCorner * -1, 0);
        else
            dieOverlayDie.transform.position = overlayCam.ScreenToWorldPoint(new Vector3(overlayCam.pixelWidth, overlayCam.pixelHeight, overlayCam.nearClipPlane + 2)) + new Vector3(distanceFromCorner * -1, distanceFromCorner * -1, 0);
        updateInvisibleFaces();
    }

    /// <summary>
    /// Sets the position of the die overlay on the screen. Should use with Vector2.left or Vector2.right for best results.
    /// </summary>
    /// <param name="side"></param>
    public void SetSide(Vector2 side2Set)
    {
        side = side2Set;
    }

    public IEnumerator RollOverlay(Vector3 axis, float rollSpeed)
    {
        // shmoove the die
        Vector3 anchor = overlayDie.transform.position;
        for (int i = 0; i < (90 / rollSpeed); i++)
        {
            overlayDie.transform.RotateAround(anchor, axis, rollSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        updateInvisibleFaces();


    }

    /// <summary>
    /// Updates the sprites representing faces that can't be seen from the current camera angle
    /// </summary>
    public void updateInvisibleFaces() {
        Dictionary<Vector3Int, int> invisPips = die.GetInvisibleFaces();
        hiddenBottomPip.sprite = pipIcons[invisPips[Vector3Int.up] - 1];
        hiddenRightPip.sprite = pipIcons[invisPips[Vector3Int.right] - 1];
        hiddenLeftPip.sprite = pipIcons[invisPips[Vector3Int.forward] - 1];
    }

    /// <summary>
    /// adds charge model to overlay
    /// </summary>
    /// <param name="type"></param>
    /// <param name="direction"></param>
    public void PowerUp(Mesh toAdd, Material meshMaterial)
    {
        chargeFaceObj.transform.position = dieOverlayDie.transform.position+ new Vector3(0, -.185f, 0);
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
}
