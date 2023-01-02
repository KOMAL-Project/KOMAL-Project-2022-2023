using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipFilterController : MonoBehaviour
{
    public int pips;
    public GameObject player;
    private ManageGame mg;
    public Texture2D[] topTextures = new Texture2D[7];
    private readonly Sprite[] topSprites = new Sprite[7];

    public bool activated = false;
    private void Start()
    {
        mg = FindObjectOfType<ManageGame>();
        player = GameObject.FindGameObjectWithTag("Player");
        // set up sprites

        if (pips > 0)
        {
            Rect rect = new Rect(0, 0, 10, 10);
            topSprites[pips - 1] = Sprite.Create(topTextures[pips - 1], rect, new Vector2(.5f, .5f));

            SpriteRenderer spr = GetComponent<SpriteRenderer>();
            spr.sprite = topSprites[pips - 1];
            spr.gameObject.transform.localScale *= 10;
        }
        else GetComponent<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// Returns true if the pip count of the current face touching the the ground of die
    /// matches "pips"
    /// </summary>
    /// <param name="die"></param>
    /// <returns></returns>
    public bool MeetsPipRequirement(GameObject die)
    {
        DieController dc = die.GetComponentInChildren<DieController>();
        // because the bottom pip value may be altered by a charge,
        // we use the fact that opposing sides of the die add up to 7
        // to determine the bottom face value using the top face value.
        return (7 - dc.sides[Vector3Int.up]) == pips || pips == 0; 
    }

    public Sprite GetSprite(int type) {
        return topSprites[type - 1];
    }
}
