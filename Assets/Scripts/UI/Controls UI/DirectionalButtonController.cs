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
    [SerializeField] GameObject dPadObject;
    // variables needed for touch swiping
    Vector2[] touchStarts;
    Vector2 touchDiffStart;
    private int touchSwipeThreshold = 200, mouseSwipeThreshold = 100;

    CameraScript cs;
    ToggleButtonUIController cameraButton;


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
            { "goto-overhead", false },
            { "goto-isometric", false }
        };

        touchStarts = new Vector2[11]; // 10 fingers on two hands + 1 to store mouse input

        cs = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        cameraButton = transform.GetChild(0).GetComponent<ToggleButtonUIController>();
    }


    void Update()
    {
        // Handles touch/mouse input for detecting swipes or taps
        int touchCount = Input.touchCount;
        //Debug.Log(touchCount);
        keys["generic-touch"] = false;
        for (int i = 0; i < Input.touchCount; i++)
        {
            // Touchscreen Input
            Touch t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began)
            {
                touchStarts[i] = t.position;
                //Debug.Log("Touch Started @ " + t.position.x);
                //Debug.Log(touchStarts[i].x);

                if(Input.touchCount >= 2) touchDiffStart = touchStarts[touchCount-1] - touchStarts[touchCount-2];
            }
            if (t.phase == TouchPhase.Ended)
            {
                keys["generic-touch"] = true;

                float deltaX = t.position.x - touchStarts[i].x;
                
                // Multitouch pinch/spread detection
                //if(touchCount >= 2) // pinch/spread detection
                //{
                //    Vector2 touchDiffEnd = Input.GetTouch(touchCount - 1).position - Input.GetTouch(touchCount - 2).position;
                    
                //    float deltaMag = touchDiffEnd.magnitude - touchDiffStart.magnitude;
                //    Debug.Log(deltaMag + " " + touchSwipeThreshold);
                //    if (Mathf.Abs(deltaMag) > touchSwipeThreshold)
                //    {
                //        if (deltaMag > 0) iso = true;
                //        else overhead = true;
                //        touchStarts[i] = new Vector2();
                //        continue;
                //    }
                //}

                if (Mathf.Abs(deltaX) > touchSwipeThreshold)
                {
                    //Debug.Log("Touch " + deltaX);
                    //Debug.Log(t.position.x + " " + touchStarts[i].x + " " + deltaX);
                    string key = (deltaX < 0) ? "counterclockwise" : "clockwise";
                    keys[key] = true;
                    //Debug.Log((deltaX < 0) ? "counterclockwise" : "clockwise");
                    StartCoroutine(UncheckInput(key, .1f));
                }
                touchStarts[i] = new Vector2();
            }

            
        }
        // Mouse "touch" Input
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("MouseDown");
            touchStarts[10] = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("MouseUp");
            keys["generic-touch"] = true;

            Vector2 mousePos = Input.mousePosition;

            float deltaX = mousePos.x - touchStarts[10].x;
           
            if (Mathf.Abs(deltaX) > mouseSwipeThreshold)
            {
                //Debug.Log("Mouse " + deltaX);
                //Debug.Log(mousePos.x + " " + touchStarts[10].x + " " + deltaX);
                string key = (deltaX < 0) ? "counterclockwise" : "clockwise";
                keys[key] = true;
                //Debug.Log((deltaX < 0) ? "counterclockwise" : "clockwise");
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

        cameraButton.img.sprite = cameraButton.isPressed ? cameraButton.pressed : (!doIso ? cameraButton.on : cameraButton.off);
    }

    private IEnumerator UncheckInput(string key, float delay)
    {
        //Debug.Log("waiting to uncheck");
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
            if (cs.CanMoveCamera())
            {
                Debug.Log("Switching Iso");
                doIso = !doIso;

                if (doIso) iso = true;
                else overhead = true;
                dPadObject.GetComponent<Animator>().SetBool("Overhead", !iso);
            }
        }
        else if (input == "pause" && !ButtonScript.moving)
        {
            //prob could save this obj
            GameObject.FindWithTag("Menu").GetComponentInChildren<LevelMenuScript>().ChangeMenu(1);
        }
    }
    public void Release(string input)
    {
        if (keys.ContainsKey(input)) keys[input] = false;
        Debug.Log("ASFASFA" + doIso);
        //cameraButton.SetImage(doIso);
    }

}
