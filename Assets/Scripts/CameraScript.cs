using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraScript : MonoBehaviour
{

    public GameObject inputObj;
    private DirectionalButtonController input;
    [SerializeField]
    private KeyCode leftKey = KeyCode.Q;
    [SerializeField]
    private KeyCode rightKey = KeyCode.E;
    [SerializeField]
    private KeyCode overheadKey = KeyCode.Space;

    private GameObject player;

    private bool followPlayer;

    public int xAngle = 60;

    public Material wallMat;

    private GameObject cam;
    private Camera viewCam;

    [SerializeField]
    private float rotationSpeed, moveSpeed, zoomInSize, zoomOutSize;
    [SerializeField]
    private float delayTime = 0.25f;
    private float timeDiff;

    private float targetZoom, targetYRotation, targetXRotation = -60;

    private Vector3 targetPosition, defaultPosition;

    public int side = 2;
    


    void Start()
    {
        followPlayer = true;
        defaultPosition = new Vector3(-0.5f, 0, 0);
        targetPosition = new Vector3();

        inputObj = GameObject.FindGameObjectWithTag("D-Pad");
        input = inputObj.GetComponent<DirectionalButtonController>();
        side = 2;
        targetYRotation = transform.eulerAngles.y;
        timeDiff = 0.0f;
        cam = Camera.main.gameObject;
        viewCam = Camera.main;
        targetXRotation = -xAngle;
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>().gameObject; 

        UniversalAdditionalCameraData camData = Camera.main.GetUniversalAdditionalCameraData();
        //camData.cameraStack.Add(GameObject.FindGameObjectWithTag("OverlayCamera").GetComponent<Camera>());
    }

    private void Update()
    {
        if ((Input.GetKeyDown(leftKey) || input.keys["counterclockwise"]) && Time.time >= timeDiff) {
            timeDiff = Time.time + delayTime;
            targetYRotation -= 90;
            side++;
            Debug.Log("SIDE: " + side);
            if (targetYRotation < 0) {
                targetYRotation += 360;
            }
        }

        if ((Input.GetKeyDown(rightKey) || input.keys["clockwise"] ) && Time.time >= timeDiff)
        {
            timeDiff = Time.time + delayTime;
            targetYRotation += 90;
            side--;
            Debug.Log("SIDE: " + side);
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
        }
        if (Input.GetKeyDown(overheadKey) || input.overhead) // Set to overhead view
        {
            targetYRotation -= 30;
            targetXRotation = -90;
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
            followPlayer = false;
        }

        if (Input.GetKeyUp(overheadKey) || input.iso) // Set to isometric view
        {
            targetYRotation += 30;
            targetXRotation = -xAngle;
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
            followPlayer = true;
        }

        if (side < 0) side = 3;
        if (side > 3) side = 0;

        // Position Stuff
        targetPosition = followPlayer ? player.transform.position : defaultPosition;
        targetZoom = followPlayer ? zoomInSize : zoomOutSize;
        //Debug.Log(targetPosition);


        transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, targetXRotation, Time.deltaTime * rotationSpeed), Mathf.LerpAngle(transform.eulerAngles.y, targetYRotation, Time.deltaTime * rotationSpeed), transform.eulerAngles.z);
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosition.x, Time.deltaTime * rotationSpeed), 0, Mathf.Lerp(transform.position.z, targetPosition.z, Time.deltaTime * rotationSpeed));
        viewCam.orthographicSize = Mathf.Lerp(viewCam.orthographicSize, targetZoom, Time.deltaTime * rotationSpeed);

    }
}
