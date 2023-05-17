using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using System;

public class FrameRateController : MonoBehaviour
{
    public bool lockToFast = false; // if true framerate is locked to fast interval
    public int fastFrameInterval = 1, slowFrameInterval = 20;
    float showtime; // for how much longer the fps is uncapped.
    void Start()
    {
        OnDemandRendering.renderFrameInterval = 100;
        showtime = 3;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.P)) SetFullFPSTime(5);
        if (showtime > 0 && !lockToFast) showtime -= Time.deltaTime;
        OnDemandRendering.renderFrameInterval = (showtime > 0 || lockToFast)? fastFrameInterval: slowFrameInterval;
    }
    
    /// <summary>
    /// Sets the current FullFPSTime to toSet only if toSet is larger.
    /// </summary>
    /// <param name="toSet"></param>
    public void SetFullFPSTime(float toSet)
    {
        showtime = Mathf.Max(showtime, toSet);
    }
}
    
