using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for Lego Switches.
/// <para> State 0: Switch is active and can be pressed. Gates are up. This is default. </para>
/// <para> State 1: Switch is not active and cannot be pressed. Gates are down. </para>
/// </summary>
public class LegoSwitchController : Mechanic
{
    public List<LegoSwitchController> controllers;
    public List<GameObject> gates;
    public List<Vector2Int> gatePos;
    public Texture2D[] topTextures = new Texture2D[7];
    private readonly Sprite[] topSprites = new Sprite[7];


    private void Start()
    {
        // set up sprites
        Rect rect = new Rect(0, 0, 10, 10);    
        topSprites[type-1] = Sprite.Create(topTextures[type-1], rect, new Vector2(.5f, .5f));
        
        SpriteRenderer spr = GetComponentInChildren<SpriteRenderer>();
        spr.sprite = topSprites[type - 1];
        spr.gameObject.transform.localScale *= 10;

        state = 0;

    }

    public override void CheckForActivation()
    {
        if (state == 0 && CheckPipFilter() && position == dieControl.position)
        {
            setState(1);
            foreach (LegoSwitchController controller in controllers) if (controller != this) setState(1);
   
        }
    } 

    public override void setState(int input) {
    state = input;

    if (input == 1) {
        foreach (GameObject w in gates) w.GetComponentInChildren<Animator>().SetBool("Active", false);
        for (int i = 0; i < gates.Count; i++) gameManager.levelData[gatePos[i].x, gatePos[i].y] = null;
        foreach(SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()) {s.sprite = topSprites[6];}
    }
    else if (input == 0) {
        foreach (GameObject w in gates) w.GetComponentInChildren<Animator>().SetBool("Active", true);
        for (int i = 0; i < gates.Count; i++) gameManager.levelData[gatePos[i].x, gatePos[i].y] = gates[i];

        SpriteRenderer[] spriteRenders = GetComponentsInChildren<SpriteRenderer>();
        spriteRenders[0].sprite = topSprites[type - 1];
        spriteRenders[1].sprite = pipFilter.GetSprite(pipFilter.pips);
    }
    else Debug.Log("Something went wrong!");
    }


}
