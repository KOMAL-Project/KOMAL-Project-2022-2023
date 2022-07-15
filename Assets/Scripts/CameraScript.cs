using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private KeyCode leftKey = KeyCode.Q;
    [SerializeField]
    private KeyCode rightKey = KeyCode.E;

    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float delayTime = 0.25f;
    private float timeDiff;

    private float targetYRotation;

    public int side = 0;

    void Start()
    {
        targetYRotation = transform.eulerAngles.y;
        timeDiff = 0.0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(leftKey) && Time.time >= timeDiff) {
            timeDiff = Time.time + delayTime;
            targetYRotation -= 90;
            side--;
            if (targetYRotation < 0) {
                targetYRotation += 360;
            }
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y - 90) % 360, transform.eulerAngles.z);
        }

        if (Input.GetKeyDown(rightKey) && Time.time >= timeDiff)
        {
            timeDiff = Time.time + delayTime;
            targetYRotation += 90;
            side++;
            if (targetYRotation > 360)
            {
                targetYRotation -= 360;
            }
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 90) % 360, transform.eulerAngles.z);
        }

        if (side < 0) side = 3;
        if (side > 3) side = 0;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.LerpAngle(transform.eulerAngles.y, targetYRotation, Time.deltaTime * rotationSpeed), transform.eulerAngles.z);
    }
}
