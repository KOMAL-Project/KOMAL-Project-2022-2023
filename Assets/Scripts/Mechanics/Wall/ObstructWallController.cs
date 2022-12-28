using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructWallController : MonoBehaviour
{
    Renderer r;
    Color toAdjust;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponentInChildren<Renderer>();
    }

    public void BecomeTransparent() 
    {
        toAdjust = r.material.color;
        toAdjust.a = 0;
        r.material.SetColor("_Color", toAdjust);
    }

    public void BecomeOpaque() 
    {
        toAdjust = r.material.color;
        toAdjust.a = 1;
        r.material.SetColor("_Color",toAdjust);
    }

}
