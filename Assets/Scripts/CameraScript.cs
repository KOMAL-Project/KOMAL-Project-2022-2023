using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraScript : MonoBehaviour
{

    // Inputs
    public GameObject inputObj;
    private DirectionalButtonController input;
    [SerializeField]
    private KeyCode leftKey = KeyCode.Q;
    [SerializeField]
    private KeyCode rightKey = KeyCode.E;
    [SerializeField]
    private KeyCode overheadKey = KeyCode.Space;
    

    private GameObject player;
    private DieController die;

    private bool followPlayer;

    public int xAngle = 60;

    public Material wallMat;

    private GameObject cam;
    private Camera viewCam;

    // Movement
    [SerializeField]
    private float rotationSpeed, moveSpeed, zoomInSize, zoomOutSize;
    [SerializeField]
    private float delayTime = 0.25f;
    private float timeDiff;

    private float targetZoom, targetYRotation, targetXRotation;

    private Vector3 targetPosition, defaultPosition;
    private Vector3 cameraOffset = Vector3.zero;
    [SerializeField]
    private float cameraOffsetMultiplier;

    public int side = 2;
    public float xOffsetRotation = 60;

    DieOverlayController doc;
    GameObject dieOverlayAnchor;

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
        die = player.GetComponent<DieController>();

        UniversalAdditionalCameraData camData = Camera.main.GetUniversalAdditionalCameraData();
        camData.cameraStack.Add(GameObject.FindGameObjectWithTag("OverlayCamera").GetComponentInChildren<Camera>());

        // Die Overlay Stuff
        GameObject dieOverlayParent = GameObject.FindGameObjectWithTag("DieOverlay");
        doc = GameObject.FindGameObjectWithTag("DieOverlay").GetComponent<DieOverlayController>();
        dieOverlayAnchor = doc.overlayDie.transform.parent.gameObject;

    }

    private void Update()
    {
        if ((Input.GetKeyDown(rightKey) || input.keys["counterclockwise"]) && CanMoveCamera()) {
            timeDiff = Time.time + delayTime;
            targetYRotation -= 90;
            ChangeSide(1);
            Debug.Log("SIDE: " + side);
            if (targetYRotation < 0) {
                targetYRotation += 360;
            }
            // Die Overlay rolling
            var overlayAxis = Quaternion.Euler(0, 180 - 45, 0) * Vector3.up;
            //StartCoroutine(doc.RollOverlay(overlayAxis, 4.5f));
            doc.TurnClockwise();
        }

        if ((Input.GetKeyDown(leftKey) || input.keys["clockwise"]) && CanMoveCamera())
        {
            timeDiff = Time.time + delayTime;
            targetYRotation += 90;
            ChangeSide(-1);
            Debug.Log("SIDE: " + side);
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
            doc.TurnCounterClockwise();
            //StartCoroutine(doc.RollOverlay(overlayAxis, 4.5f));
        }
        if ((Input.GetKeyDown(overheadKey) || input.overhead) && CanMoveCamera()) // Set to overhead view
        {
            timeDiff = Time.time + delayTime;
            targetYRotation -= 30;
            targetXRotation = -90;
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
            followPlayer = false;
            doc.switchToOverhead();
        }

        if ((Input.GetKeyUp(overheadKey) || input.iso) && CanMoveCamera()) // Set to isometric view
        {
            timeDiff = Time.time + delayTime;
            targetYRotation += 30;
            targetXRotation = -xAngle;
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
            followPlayer = true;
            doc.switchOutOfOverhead();
        }


        // Position Stuff
        targetPosition = followPlayer ? player.transform.position : defaultPosition;
        targetZoom = followPlayer ? zoomInSize : zoomOutSize;
        //Debug.Log(targetPosition);

        // move things
        transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, targetXRotation, Time.deltaTime * rotationSpeed), Mathf.LerpAngle(transform.eulerAngles.y, targetYRotation, Time.deltaTime * rotationSpeed), transform.eulerAngles.z);
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosition.x, Time.deltaTime * moveSpeed), 0, Mathf.Lerp(transform.position.z, targetPosition.z, Time.deltaTime * moveSpeed));
        viewCam.orthographicSize = Mathf.Lerp(viewCam.orthographicSize, targetZoom, Time.deltaTime * rotationSpeed);
        cam.transform.localPosition = new Vector3(Mathf.Lerp(cam.transform.localPosition.x, cameraOffset.x, Time.deltaTime * rotationSpeed), 0, 50);
        

    }

    void ChangeSide(int val)
    {
        side += val;
        if (side < 0) side = 3;
        if (side > 3) side = 0;
    }

    public void SetOffset(Vector3 newSide)
    {
        cameraOffset = newSide * cameraOffsetMultiplier;
    }

    public float GetTimeDiff() { return timeDiff; }

    public bool CanMoveCamera()
    {
        return Time.time >= timeDiff && !die.getIsMoving();
    }
}
