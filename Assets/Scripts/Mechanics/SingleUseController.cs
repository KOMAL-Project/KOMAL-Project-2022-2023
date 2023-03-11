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
    }

    public override void CheckForActivation() {

        Vector2Int playerPosition = dieControl.position;

        if (playerPosition == position) state = 1;
        if (playerPosition != position && state == 1)
        {
            gameManager.levelData[position.x, position.y] = this.gameObject;
            state = 2;
            LeanTween.moveLocalY(block, 0.5f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }
    }

    public override void SetState(int input) {
        state = input;
        if (input < 2 && state == 1) {
            gameManager.levelData[position.x, position.y] = null;

            LeanTween.cancel(block);
            Vector3 localpos = block.transform.localPosition; //conversion of the local y position to 1. There could be a better way to do this
            localpos.y = 15;
            block.transform.localPosition = localpos;
        }
        
    }
   
}
