using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FaceSwitchController : MonoBehaviour
{

    public int pips;
    public List<GameObject> walls;
    public List<Vector2Int> wallsPos;
    public Vector2Int thisPos;
    public Vector2Int playerPos;
    public GameObject player;


    private ManageGame mg;

    public Texture2D[] topTextures = new Texture2D[7];
    private Sprite[] topSprites = new Sprite[7];


    private void Start()
    {

        // set up sprites
        for (int i = 0; i < 6; i++)
        {
            Rect rect = new Rect(0, 0, 10, 10);
            topSprites[i] = Sprite.Create(topTextures[i], rect, new Vector2(.5f, .5f));
        }
        

        player = GameObject.FindGameObjectWithTag("Player");

        mg = FindObjectOfType<ManageGame>();

        GetComponentInChildren<SpriteRenderer>().sprite = topSprites[pips-1];

    }

    private void Update()
    {
        playerPos = player.GetComponentInChildren<DieController>().position;
        //Debug.Log(playerPos + " // "  + thisPos );
        if (thisPos == playerPos && player.GetComponentInChildren<DieController>().sides[Vector3.down] == pips)
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAA");

            foreach (GameObject w in walls) 
            {
                w.GetComponentInChildren<Animator>().SetBool("Active", false);
            }

            for (int i = 0; i < walls.Count; i++) 
            {
                mg.levelData[wallsPos[i].x, wallsPos[i].y] = null;
            }
            GetComponentInChildren<SpriteRenderer>().sprite = topSprites[6];
   
        }
    }
}
