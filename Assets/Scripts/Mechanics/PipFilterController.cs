using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipFilterController : MonoBehaviour
{
    public int pips;
    public Texture2D[] topTextures = new Texture2D[7];
    private readonly Sprite[] topSprites = new Sprite[7];
    public SpriteRenderer spriteRenderer;
    private LTSeq seq;
    private void Start()
    {

        if (pips > 0)
        {
            Rect rect = new Rect(0, 0, 10, 10);
            topSprites[pips - 1] = Sprite.Create(topTextures[pips - 1], rect, new Vector2(.5f, .5f));

            spriteRenderer.sprite = topSprites[pips - 1];
        }
        else 
        {
        Disable();
        DisablePulse();
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

    public void Enable() {
        spriteRenderer.enabled = true;
    }
    public void Disable() {
        spriteRenderer.enabled = false;
    }
    public void EnablePulse() 
    {
        if (!LeanTween.isTweening(gameObject) && pips > 0)
        {
            StartCoroutine(Pulse());
        }
    }
    public void DisablePulse()
    {
        LeanTween.cancel(gameObject);
        transform.localScale = Vector3.one * 10;
        spriteRenderer.material.color = Color.white;
    }

    public IEnumerator Pulse() { //begins the pulsing
        yield return new WaitForSeconds(0.5f);

        seq = LeanTween.sequence();
        seq.append(LeanTween.scale(gameObject, Vector3.one * 10, 0.75f).setEaseInOutQuad());
        seq.insert(LeanTween.alpha(gameObject, 0, 0.75f).setEaseInOutQuad());
        seq.append(LeanTween.scale(gameObject, Vector3.one * 5, 0.75f).setEaseInOutQuad().setDelay(1.2f));
        seq.insert(LeanTween.alpha(gameObject, 1, 0.75f).setEaseInOutQuad());
        seq.append(LeanTween.value(gameObject, transform.position, transform.position, 0f).setOnComplete( () => { //repeats the whole sequence
            StartCoroutine(Pulse());
        }).setDelay(0.05f));
    }

    private void Update() {
    }

}
