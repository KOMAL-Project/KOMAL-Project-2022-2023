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

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponentInChildren<TextMeshPro>().text = pips.ToString();
        mg = FindObjectOfType<ManageGame>();
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
                w.GetComponent<Animator>().SetBool("Active", false);
            }

            for (int i = 0; i < walls.Count; i++) 
            {
                mg.levelData[wallsPos[i].x, wallsPos[i].y] = null;
            }
            
        }


    }




}
