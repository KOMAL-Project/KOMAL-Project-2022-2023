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
            SetState(1);
            foreach (LegoSwitchController controller in controllers) if (controller != this) SetState(1);
   
        }
    } 

    public override void SetState(int input) {

    if (input == 1 &&  state != 1) {
        foreach (GameObject w in gates) LeanTween.moveLocalY(w, 50, 1).setEase(LeanTweenType.easeInOutQuad);
        for (int i = 0; i < gates.Count; i++) gameManager.levelData[gatePos[i].x, gatePos[i].y] = null;
        foreach(SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()) {s.sprite = topSprites[6];}
    }
    else if (input == 0 && state != 0) {
        foreach (GameObject w in gates) 
        {
        LeanTween.cancel(w);
        Vector3 localpos = w.transform.localPosition; //conversion of the local y position to 1. There could be a better way to do this
        localpos.y = 1;
        w.transform.localPosition = localpos;
        }
        
        for (int i = 0; i < gates.Count; i++) gameManager.levelData[gatePos[i].x, gatePos[i].y] = gates[i];

        SpriteRenderer[] spriteRenders = GetComponentsInChildren<SpriteRenderer>();
        spriteRenders[0].sprite = topSprites[type - 1];
        if (spriteRenders[1].sprite != null) spriteRenders[1].sprite = pipFilter.GetSprite(pipFilter.pips);
    }
    state = input;
    }


}
