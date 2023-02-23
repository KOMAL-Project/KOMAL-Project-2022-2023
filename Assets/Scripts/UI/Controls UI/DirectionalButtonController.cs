using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mono.Cecil;

public class DirectionalButtonController : MonoBehaviour
{
    // Standard Inputs
    public Dictionary<string, bool> keys;
    // Toggle button vars
    public bool overhead, iso, doIso;

    [SerializeField] bool showUI;
    [SerializeField] GameObject dPadObject, overheadButton;
    CameraScript cs;
    // variables needed for touch swiping
    Vector2[] touchStarts;
    private int touchSwipeThreshold = 200, mouseSwipeThreshold = 100;


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
            { "generic-touch", false },
        };

        touchStarts = new Vector2[11]; // 10 fingers on two hands + 1 to store mouse input
        cs = Camera.main.gameObject.GetComponentInParent<CameraScript>();
        overheadButton = dPadObject.transform.parent.GetChild(0).gameObject;
    }

    private Vector3 startPosition = Vector3.zero;
    private Vector3 endPosition = Vector3.zero;

    void Update()
    {
        // Handles touch/mouse input for detecting swipes or taps

        keys["generic-touch"] = false;
        for (int i = 0; i < Input.touchCount; i++)
        {
            // Touchscreen Input
            Touch t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began)
            {
                touchStarts[i] = t.position;
                Debug.Log("Touch Started @ " + t.position.x);
                Debug.Log(touchStarts[i].x);
            }
            if (t.phase == TouchPhase.Ended)
            {
                keys["generic-touch"] = true;

                float deltaX = t.position.x - touchStarts[i].x;
                
                if (Mathf.Abs(deltaX) > touchSwipeThreshold)
                {
                    Debug.Log("Touch " + deltaX);
                    Debug.Log(t.position.x + " " + touchStarts[i].x + " " + deltaX);
                    string key = (deltaX < 0) ? "counterclockwise" : "clockwise";
                    keys[key] = true;
                    Debug.Log((deltaX < 0) ? "counterclockwise" : "clockwise");
                    StartCoroutine(UncheckInput(key, .1f));
                }
                touchStarts[i] = new Vector2();
            }
        }
        // Mouse "touch" Input
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("MouseUp");
            touchStarts[10] = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("MouseUp");
            keys["generic-touch"] = true;

            Vector2 mousePos = Input.mousePosition;

            float deltaX = mousePos.x - touchStarts[10].x;
           
            if (Mathf.Abs(deltaX) > mouseSwipeThreshold)
            {
                Debug.Log("Mouse " + deltaX);
                Debug.Log(mousePos.x + " " + touchStarts[10].x + " " + deltaX);
                string key = (deltaX < 0) ? "counterclockwise" : "clockwise";
                keys[key] = true;
                Debug.Log((deltaX < 0) ? "counterclockwise" : "clockwise");
                StartCoroutine(UncheckInput(key, .1f));
            }
            touchStarts[10] = new Vector2();
        }
    }

    private void LateUpdate()
    {
        //Debug.Log(iso + " " + overhead + " " + doIso);
        iso = false;
        overhead = false;
        keys["undo"] = false;
    }

    private IEnumerator UncheckInput(string key, float delay)
    {
        Debug.Log("waiting to uncheck");
        yield return new WaitForSeconds(delay);
        Release(key);
        //Debug.Log(key + " released");
    }

    
    public void Press(string input)
    {
        if (keys.ContainsKey(input)) keys[input] = true;
        
        // Have to do a separate thing for toggle button presses
        if (input == "camera")
        {
            if (!cs.CanMoveCamera()) 
            {
                overheadButton.GetComponent<ToggleButtonUIController>().Release();
                return; 
            }
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
