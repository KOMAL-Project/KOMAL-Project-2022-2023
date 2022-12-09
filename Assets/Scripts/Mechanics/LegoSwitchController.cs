using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LegoSwitchController : Mechanic
{

    public List<GameObject> walls;
    public List<Vector2Int> wallsPos;
    public Vector2Int playerPos;
    private bool active = true;

    public Texture2D[] topTextures = new Texture2D[7];
    private readonly Sprite[] topSprites = new Sprite[7];


    private void Start()
    {
        // set up sprites
        Rect rect = new Rect(0, 0, 10, 10);    
        topSprites[type-1] = Sprite.Create(topTextures[type-1], rect, new Vector2(.5f, .5f));
        
        // Debug.Log(pips + " " + type);
        active = true;
        

        SpriteRenderer spr = GetComponentInChildren<SpriteRenderer>();
        spr.sprite = topSprites[type - 1];
        spr.gameObject.transform.localScale *= 10;

    }

    public override void CheckForActivation()
    {
        //Debug.Log(playerPos + " // "  + thisPos );
        if (active != false && CheckPipFilter() && position == dieControl.position)
        {
            Debug.Log("Face switch triggered!");
            active = false;

            foreach (GameObject w in walls) 
            {
                w.GetComponentInChildren<Animator>().SetBool("Active", false);
            }

            for (int i = 0; i < walls.Count; i++) 
            {
                gameManager.levelData[wallsPos[i].x, wallsPos[i].y] = null;
            }
            foreach(SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()) {s.sprite = topSprites[6];}
   
        }
    }

    public byte getStateByte() {
        if (active) return 1;
        if (!active) return 0;
        else {Debug.Log("Something went wrong!"); return 50;}
    }

    public void ByteToSetState(byte input) {
        if (input == 0) {
            active = false;
            foreach (GameObject w in walls) w.GetComponentInChildren<Animator>().SetBool("Active", false);
            for (int i = 0; i < walls.Count; i++) gameManager.levelData[wallsPos[i].x, wallsPos[i].y] = null;
        }
        else if (input == 1) {
            if (!active) { //resets the walls if it wasnt active and now is again
                active = true;
                foreach (GameObject w in walls) w.GetComponentInChildren<Animator>().SetBool("Active", true);
                for (int i = 0; i < walls.Count; i++) gameManager.levelData[wallsPos[i].x, wallsPos[i].y] = walls[i];
                SpriteRenderer[] spriteRenders = GetComponentsInChildren<SpriteRenderer>();
                spriteRenders[0].sprite = topSprites[type - 1];
                spriteRenders[1].sprite = pipFilter.GetSprite(pipFilter.pips);

            }
        }
        else Debug.Log("Something went wrong!");
    }
}
