using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for Single Use Tiles.
/// <para> State 0: Tile is not activated or primed. This is default. </para>
/// <para> State 1: Tile is not activated but primed to fall once die leaves. </para>
/// <para> State 2: Tile has been activated. </para>
/// </summary>
public class SingleUseController : Mechanic
{
    private GameObject block;

    // Start is called before the first frame update
    void Start()
    {
        state = 0;
        block = transform.GetChild(0).gameObject;
        GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    public override void CheckForActivation() {

        Vector2Int playerPosition = dieControl.position;

        // Check if player is on tile
        if (playerPosition == position) state = 1;
        // Check when player leaves tile
        if (playerPosition != position && state == 1)
        {
            GetComponentInChildren<MeshRenderer>().enabled = true;
            gameManager.levelData[position.x, position.y] = this.gameObject;
            state = 2;
            Vector3 localpos = block.transform.localPosition;
            localpos.y = 15;
            block.transform.localPosition = localpos;
            LeanTween.moveLocalY(block, 0.5f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }
    }

    // 0 -- block is in the air and player has not primed it yet
    // 1 -- player on tile, block is primed to fall once player steps off
    // 2 -- player has walked off and the block is falling
    public override void SetState(int input) {
        state = input;
        if (input < 2 && state == 1) { // reset the block
            gameManager.levelData[position.x, position.y] = null;

            LeanTween.cancel(block);
            Vector3 localpos = block.transform.localPosition;
            localpos.y = -200;
            block.transform.localPosition = localpos;
            // make block invis again
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
        
    }


}
