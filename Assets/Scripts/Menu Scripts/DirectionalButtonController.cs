using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalButtonController : MonoBehaviour
{
    public bool up, down, left, right, counterclockwise, clockwise;

    private void Start() {
        GetComponent<UnityEngine.UI.Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
    
    public void Press(string direction)
    {
        if (direction == "up") up = true;
        if (direction == "down") down = true;
        if (direction == "left") left = true;
        if (direction == "right") right = true;
        if (direction == "clockwise") clockwise = true;
        if (direction == "counterclockwise") counterclockwise = true;

    }
    public void Release(string direction)
    {
        if (direction == "up") up = false;
        if (direction == "down") down = false;
        if (direction == "left") left = false;
        if (direction == "right") right = false;
        if (direction == "clockwise") clockwise = false;
        if (direction == "counterclockwise") counterclockwise = false;
    }

}
