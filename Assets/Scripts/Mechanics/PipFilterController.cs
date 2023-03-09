using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipFilterController : MonoBehaviour
{
    public int pips;
    public Texture2D[] topTextures = new Texture2D[7];
    private readonly Sprite[] topSprites = new Sprite[7];
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (pips > 0)
        {
            Rect rect = new Rect(0, 0, 10, 10);
            topSprites[pips - 1] = Sprite.Create(topTextures[pips - 1], rect, new Vector2(.5f, .5f));

            spriteRenderer.sprite = topSprites[pips - 1];
            spriteRenderer.gameObject.transform.localScale *= 10;
            animator.Play("Normal");
        }
        else 
        {
        spriteRenderer.enabled = false;
        animator.enabled = false;
        }
    }

    /// <summary>
    /// Returns true if the pip count inputed
    /// matches "pips"
    /// </summary>
    /// <param name="die"></param>
    /// <returns></returns>
    public bool MeetsPipRequirement(int comparedPip)
    {
        return comparedPip == pips || pips == 0;
    }

    public Sprite GetSprite(int type) 
    {
        return topSprites[type - 1];
    }

    public void Enable() 
    {
        spriteRenderer.enabled = true;
        animator.enabled = true;
    }

    public void Disable() {
        spriteRenderer.enabled = false;
        animator.enabled = false;
    }
}
