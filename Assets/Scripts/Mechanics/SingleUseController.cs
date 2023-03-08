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

    // Start is called before the first frame update
    void Start()
    {
        state = 0;
    }

    public override void CheckForActivation() {

        Vector2Int playerPosition = dieControl.position;

        if (playerPosition == position) state = 1;
        if (playerPosition != position && state == 1)
        {
            gameManager.levelData[position.x, position.y] = this.gameObject;
            state = 2;
            GetComponentInChildren<Animator>().SetTrigger("Go");
        }
    }

    public override void SetState(int input) {
        state = input;
        if (input == 0) {
            GetComponentInChildren<Animator>().SetTrigger("Back");
            gameManager.levelData[position.x, position.y] = null;
        }
    }
   
}
