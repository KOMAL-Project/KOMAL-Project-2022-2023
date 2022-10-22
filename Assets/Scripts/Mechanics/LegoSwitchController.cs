using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LegoSwitchController : MonoBehaviour
{

    public int pips, type;
    public List<GameObject> walls;
    public List<Vector2Int> wallsPos;
    public Vector2Int thisPos;
    public Vector2Int playerPos;
    public GameObject player;
    PipFilterController pip;

    private ManageGame mg;

    public Texture2D[] topTextures = new Texture2D[7];
    private readonly Sprite[] topSprites = new Sprite[7];


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        // set up sprites
        Rect rect = new Rect(0, 0, 10, 10);    
        topSprites[type-1] = Sprite.Create(topTextures[type-1], rect, new Vector2(.5f, .5f));
        
        mg = FindObjectOfType<ManageGame>();
        pip = GetComponentInChildren<PipFilterController>();
        // Debug.Log(pips + " " + type);
        pip.pips = pips;
        pip.thisPos = thisPos;
        pip.player = player;
        pip.playerPos = playerPos;
        

        SpriteRenderer spr = GetComponentInChildren<SpriteRenderer>();
        spr.sprite = topSprites[type - 1];
        spr.gameObject.transform.localScale *= 10;

    }

    private void LateUpdate()
    {
        playerPos = player.GetComponentInChildren<DieController>().position;
        //Debug.Log(playerPos + " // "  + thisPos );
        if (pip.MeetsPipRequirement(player) && thisPos == playerPos)
        {
            Debug.Log("Face switch triggered!");

            foreach (GameObject w in walls) 
            {
                w.GetComponentInChildren<Animator>().SetBool("Active", false);
            }

            for (int i = 0; i < walls.Count; i++) 
            {
                mg.levelData[wallsPos[i].x, wallsPos[i].y] = null;
            }
            foreach(SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()) s.sprite = topSprites[6];
   
        }
    }
}
