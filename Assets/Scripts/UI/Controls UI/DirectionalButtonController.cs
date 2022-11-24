using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DirectionalButtonController : MonoBehaviour
{
    // Standard Inputs
    public Dictionary<string, bool> keys;
    // Toggle button vars
    public bool overhead, iso, doIso;

    [SerializeField] bool showUI;
    [SerializeField] GameObject dPadObject;
    
    private void Start()
    {
        doIso = true;
        iso = false;
        overhead = false;
        if(showUI == false) foreach (Image i in GetComponentsInChildren<Image>()) i.enabled = false;

        // Set up dictionary of inputs
        keys = new Dictionary<string, bool>
        {
            { "up", false },
            { "down", false },
            { "left", false },
            { "right", false },
            { "counterclockwise", false },
            { "clockwise", false },
            { "undo", false },
        };
        
    }

    private void LateUpdate()
    {
        //Debug.Log(iso + " " + overhead + " " + doIso);
        iso = false;
        overhead = false;
        keys["undo"] = false;
    }

    public void Press(string input)
    {
        if (keys.ContainsKey(input)) keys[input] = true;
        
        // Have to do a separate thing for toggle button presses
        if (input == "camera")
        {
            doIso = !doIso;
            if (doIso) iso = true;
            else overhead = true;
            dPadObject.GetComponent<Animator>().SetBool("Overhead", !iso);
        }
        else if (input == "pause")
        {
            GameObject.FindWithTag("Menu").GetComponentInChildren<LevelMenuScript>().changeMenu(1);
        }


    }
    public void Release(string input)
    {
        if (keys.ContainsKey(input)) keys[input] = false;
    }

}
