using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSwitchController : MonoBehaviour
{
    ManageGame gameManager;
    DieController die;
    PipFilterController pipFilter;
    public int pips;
    public Vector2Int position;
    // switches: All toggle switches in the current level, including this one
    public List<GameObject> xBlocks, oBlocks, switches;
    public List<Vector2Int> xBlockPositions, oBlockPositions;
    string state = "x";

    // Physical Appearance
    SpriteRenderer spr;
    [SerializeField] Texture2D oTexture, xTexture;
    Sprite oSprite, xSprite;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ManageGame>();
        die = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        pipFilter = GetComponentInChildren<PipFilterController>();
        pipFilter.pips = pips;

        Rect rect = new Rect(0, 0, 10, 10);
        oSprite = Sprite.Create(oTexture, rect, new Vector2(.5f, .5f));
        xSprite = Sprite.Create(xTexture, rect, new Vector2(.5f, .5f));

        spr = GetComponent<SpriteRenderer>();
        spr.sprite = state == "x" ? xSprite : oSprite;
        spr.gameObject.transform.localScale *= 10;
        transform.rotation.Set(-90, 0, 0, 0);

        // Activate X Blocks
        for (int i = 0; i < xBlocks.Count; i++)
        {
            Vector2Int coords = xBlockPositions[i];
            gameManager.levelData[coords.x, coords.y] = xBlocks[i];
            xBlocks[i].GetComponentInChildren<Animator>().SetBool("Activated", true);
        }
    }

    // Update is called once per frame
    public void CheckForActivation()
    {
        //Debug.Log(position +"   "  + die.position);
        if(die.position == position)
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAA");
            state = state == "x" ? "o" : "x"; // swap active block
            List<GameObject> toActivate = state == "x" ? xBlocks : oBlocks;
            List<GameObject> toDeactivate = state == "x" ? oBlocks : xBlocks;
            List<Vector2Int> coordsOfToActivate = state == "x" ? xBlockPositions : oBlockPositions;
            List<Vector2Int> coordsOfToDeactivate = state == "x" ? oBlockPositions : xBlockPositions;

            // In with the new...
            for (int i = 0; i < toActivate.Count; i++)
            {
                Vector2Int coords = coordsOfToActivate[i];
                gameManager.levelData[coords.x, coords.y] = toActivate[i];
                toActivate[i].GetComponentInChildren<Animator>().SetBool("Activated", true);
            }
            // ... out with the old.
            for (int i = 0; i < toDeactivate.Count; i++)
            {
                Vector2Int coords = coordsOfToDeactivate[i];
                gameManager.levelData[coords.x, coords.y] = null;
                toDeactivate[i].GetComponentInChildren<Animator>().SetBool("Activated", false);
            }
            // Finally, update all of the switches.
            Sprite newSwitchSprite = state == "x" ? xSprite : oSprite;
            spr.sprite = newSwitchSprite;
            //foreach (GameObject s in switches) s.GetComponent<SpriteRenderer>().sprite = newSwitchSprite;
        }
    }
}
