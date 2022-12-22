using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSwitchController : Mechanic
{
    ManageGame gameManager;
    DieController die;
    PipFilterController pipFilter;
    public int pips;
    // switches: All toggle switches in the current level, including this one
    public List<GameObject> xBlocks, oBlocks, switches;
    public List<Vector2Int> xBlockPositions, oBlockPositions;
    public string state = "x";

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
            GameObject temp = xBlocks[i].transform.GetChild(0).gameObject;
            temp.GetComponent<Animator>().SetBool("Activated", true);
            temp.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("On", true);
        }

        pipFilter.gameObject.transform.localScale /= 10;
        pipFilter.gameObject.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, .01f);
    }

    public override void CheckForActivation()
    {
        //Debug.Log(position +"   "  + die.position);
        if(die.position == position && pipFilter.MeetsPipRequirement(die.gameObject))
        {
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAA");
            state = state == "x" ? "o" : "x"; // swap active block
            doToggle();
        }
    }

    public void doToggle() {
        List<GameObject> toActivate = state == "x" ? xBlocks : oBlocks;
        List<GameObject> toDeactivate = state == "x" ? oBlocks : xBlocks;
        List<Vector2Int> coordsOfToActivate = state == "x" ? xBlockPositions : oBlockPositions;
        List<Vector2Int> coordsOfToDeactivate = state == "x" ? oBlockPositions : xBlockPositions;

        // In with the new...
        for (int i = 0; i < toActivate.Count; i++)
        {
            Vector2Int coords = coordsOfToActivate[i];
            gameManager.levelData[coords.x, coords.y] = toActivate[i];
            GameObject temp = toActivate[i].transform.GetChild(0).gameObject;
            temp.GetComponent<Animator>().SetBool("Activated", true);
            temp.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("On", true);
        }
        // ... out with the old.
        for (int i = 0; i < toDeactivate.Count; i++)
        {
            Vector2Int coords = coordsOfToDeactivate[i];
            gameManager.levelData[coords.x, coords.y] = null;
            GameObject temp = toDeactivate[i].transform.GetChild(0).gameObject;
            temp.GetComponent<Animator>().SetBool("Activated", false);
            temp.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("On", false);
        }
        // Finally, update all of the switches.
        Sprite newSwitchSprite = (state == "x") ? xSprite : oSprite;
        spr.sprite = newSwitchSprite;
        foreach (GameObject s in switches) {
            s.GetComponentInChildren<SpriteRenderer>().sprite = newSwitchSprite; 
            s.GetComponentInChildren<ToggleSwitchController>().state = this.state;
        }
    }

    public bool stateToGetBool() {
        return state == "x"? true : false;
    }

    public void boolToSetState(bool setBool) {
        if (stateToGetBool() != setBool) {
            state = setBool ? "x" : "o";
            doToggle();
        }
    }
}
