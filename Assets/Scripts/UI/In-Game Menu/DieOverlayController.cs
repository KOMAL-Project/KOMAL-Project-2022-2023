using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOverlayController : MonoBehaviour
{
    GameObject dieOverlayDie;
    Camera overlayCam;

    DieController die;

    SpriteRenderer hiddenBottomPip, hiddenLeftPip, hiddenRightPip;
    [SerializeField]
    GameObject hiddenBotomPipObj, hiddenLeftPipObj, hiddenRightPipObj,
        overlayDie;

    [SerializeField] Texture2D[] pipIconTextures;
    Sprite[] pipIcons;

    // Start is called before the first frame update
    void Start()
    {
        pipIcons = new Sprite[6];
        
        for (int i = 0; i < 6; i++)
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
        Dictionary<Vector3Int, int> invisPips = die.GetInvisibleFaces();
        hiddenBottomPip.sprite = pipIcons[invisPips[Vector3Int.up] - 1];
        hiddenRightPip.sprite = pipIcons[invisPips[Vector3Int.right] - 1];
        hiddenLeftPip.sprite = pipIcons[invisPips[Vector3Int.forward] - 1];

    }

    // Update is called once per frame
    void Update()
    {
        dieOverlayDie.transform.position = overlayCam.ScreenToWorldPoint(new Vector3(0, overlayCam.pixelHeight, overlayCam.nearClipPlane + 2));
        dieOverlayDie.transform.position += new Vector3(.75f, -.75f, 0);
        Dictionary<Vector3Int, int> invisPips = die.GetInvisibleFaces();
        hiddenBottomPip.sprite = pipIcons[invisPips[Vector3Int.up] - 1];
        hiddenRightPip.sprite = pipIcons[invisPips[Vector3Int.right] - 1];
        hiddenLeftPip.sprite = pipIcons[invisPips[Vector3Int.forward] - 1];
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
        // Update the pip back things
        Dictionary<Vector3Int, int> invisPips = die.GetInvisibleFaces();
        hiddenBottomPip.sprite = pipIcons[invisPips[Vector3Int.up] - 1];
        hiddenRightPip.sprite = pipIcons[invisPips[Vector3Int.right] - 1];
        hiddenLeftPip.sprite = pipIcons[invisPips[Vector3Int.forward] - 1];
    }
}
