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

    public int xAngle = 60;

    public Material wallMat;

    private GameObject cam;
    private GameObject player;

    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float delayTime = 0.25f;
    private float timeDiff;

    private float targetYRotation, targetXRotation = -60;

    public int side = 2;


    void Start()
    {
        inputObj = GameObject.FindGameObjectWithTag("D-Pad");
        input = inputObj.GetComponent<DirectionalButtonController>();
        side = 2;
        targetYRotation = transform.eulerAngles.y;
        timeDiff = 0.0f;
        cam = Camera.main.gameObject;
        targetXRotation = -xAngle;
        player = GameObject.FindGameObjectWithTag("Player");

        UniversalAdditionalCameraData camData = Camera.main.GetUniversalAdditionalCameraData();
        camData.cameraStack.Add(GameObject.FindGameObjectWithTag("OverlayCamera").GetComponent<Camera>());
    }

    private void Update()
    {
        //Debug.Log("c.side: " + side);
        if ((Input.GetKeyDown(leftKey) || input.counterclockwise) && Time.time >= timeDiff) {
            timeDiff = Time.time + delayTime;
            targetYRotation -= 90;
            side++;
            Debug.Log("SIDE: " + side);
            if (targetYRotation < 0) {
                targetYRotation += 360;
            }
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y - 90) % 360, transform.eulerAngles.z);
        }

        if ((Input.GetKeyDown(rightKey) || input.clockwise ) && Time.time >= timeDiff)
        {
            timeDiff = Time.time + delayTime;
            targetYRotation += 90;
            side--;
            Debug.Log("SIDE: " + side);
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 90) % 360, transform.eulerAngles.z);
        }
        if (Input.GetKeyDown(overheadKey))
        {
            targetYRotation -= 30;
            targetXRotation = -90;
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 90) % 360, transform.eulerAngles.z);
        }

        if (Input.GetKeyUp(overheadKey))
        {
            targetYRotation += 30;
            targetXRotation = -xAngle;
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 90) % 360, transform.eulerAngles.z);
        }

        if (side < 0) side = 3;
        if (side > 3) side = 0;

        transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, targetXRotation, Time.deltaTime * rotationSpeed), Mathf.LerpAngle(transform.eulerAngles.y, targetYRotation, Time.deltaTime * rotationSpeed), transform.eulerAngles.z);

    }
}
