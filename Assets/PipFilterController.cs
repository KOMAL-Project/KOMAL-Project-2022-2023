using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipFilterController : MonoBehaviour
{
    public int pips;
    public Vector2Int thisPos;
    public Vector2Int playerPos;
    public GameObject player;
    
    private ManageGame mg;

    public Texture2D[] topTextures = new Texture2D[7];
    private Sprite[] topSprites = new Sprite[7];

    public bool activated = false;

    private void Start()
    {
        mg = FindObjectOfType<ManageGame>();
        player = GameObject.FindGameObjectWithTag("Player");
        // set up sprites

        Rect rect = new Rect(0, 0, 10, 10);
        topSprites[pips - 1] = Sprite.Create(topTextures[pips - 1], rect, new Vector2(.5f, .5f));
        
        SpriteRenderer spr = GetComponentInChildren<SpriteRenderer>();
        spr.sprite = topSprites[pips - 1];
        spr.gameObject.transform.localScale *= 10;

    }

    private void Update()
    {
        playerPos = player.GetComponentInChildren<DieController>().position;
        //Debug.Log(playerPos + " // "  + thisPos );
        if (thisPos == playerPos && (player.GetComponentInChildren<DieController>().sides[Vector3.down] == pips || pips == 0))
        {
            Debug.Log("Face switch triggered!");
            activated = true;
        }
    }
}
