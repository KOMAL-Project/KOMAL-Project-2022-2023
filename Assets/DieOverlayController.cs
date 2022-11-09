using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOverlayController : MonoBehaviour
{
    GameObject dieOverlayDie;
    Camera overlayCam;
    // Start is called before the first frame update
    void Start()
    {
        dieOverlayDie = transform.GetChild(2).gameObject;
        overlayCam = GetComponentInChildren<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        dieOverlayDie.transform.position = overlayCam.ScreenToWorldPoint(new Vector3(0, overlayCam.pixelHeight, overlayCam.nearClipPlane + 2));
        dieOverlayDie.transform.position += new Vector3(.75f, -.75f, 0);
    }
}
