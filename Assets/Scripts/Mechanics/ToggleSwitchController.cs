using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for ToggleSwitches.
/// <para>  State 0: X is shown, meaning O is down and X is up. This is default.   </para>
/// <para>  State 1: O is shown, meaning X is up and O is down.   </para>
/// </summary>

public class ToggleSwitchController : Mechanic
{
    // switches: All toggle switches in the current level, including this one
    public List<GameObject> xBlocks, oBlocks;
    public List<ToggleSwitchController> switches;
    public List<Vector2Int> xBlockPositions, oBlockPositions;
    // Physical Appearance
    SpriteRenderer spr;
    [SerializeField] Texture2D oTexture, xTexture;
    Sprite oSprite, xSprite;

    // Start is called before the first frame update
    void Start()
    {
        state = 0;

        Rect rect = new Rect(0, 0, 10, 10);
        oSprite = Sprite.Create(oTexture, rect, new Vector2(.5f, .5f));
        xSprite = Sprite.Create(xTexture, rect, new Vector2(.5f, .5f));

        spr = GetComponent<SpriteRenderer>();
        spr.sprite = state == 0 ? xSprite : oSprite;
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
        // Deactivate O Blocks
        for (int i = 0; i < oBlocks.Count; i++)
        {
            Vector2Int coords = oBlockPositions[i];
            gameManager.levelData[coords.x, coords.y] = null;
        }

        pipFilter.gameObject.transform.localScale /= 10;
        pipFilter.gameObject.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, .01f);
    }

    public override void CheckForActivation()
    {
        //Debug.Log(position +"   "  + dieControl.position);
        if(dieControl.position == position && CheckPipFilter())
        {
            state = state == 1 ? 0 : 1; // swap active block
            doToggle();
        }
    }

    public void doToggle() {
        List<GameObject> toActivate = state == 0 ? xBlocks : oBlocks;
        List<GameObject> toDeactivate = state == 0 ? oBlocks : xBlocks;
        List<Vector2Int> coordsOfToActivate = state == 0 ? xBlockPositions : oBlockPositions;
        List<Vector2Int> coordsOfToDeactivate = state == 0 ? oBlockPositions : xBlockPositions;

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
        Sprite newSwitchSprite = (state == 0) ? xSprite : oSprite;
        spr.sprite = newSwitchSprite;
        foreach (ToggleSwitchController s in switches) {
            s.spr.sprite = newSwitchSprite; 
            s.state = this.state;
        }
    }

    public override void setState(int input) {
        if (state != input) {
            state = input;
            doToggle();
        }
    }
}
