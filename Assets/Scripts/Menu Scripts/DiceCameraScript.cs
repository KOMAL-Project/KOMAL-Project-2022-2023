using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCameraScript : MonoBehaviour
{
    [SerializeField] private float animationDuration = 2;
    [SerializeField] private float farDistance = 20;
    private Transform trans;
    private Vector3 pos;

    private Vector3 closePos;
    private Vector3 farPos;
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
        closePos = trans.position;
        farPos = new Vector3(trans.position.x, trans.position.y, trans.position.z - farDistance);
    }

    public void pullAway() {
        StartCoroutine(pullA());
    }
    public void pullToward() {
        StartCoroutine(pullT());
    }

    private IEnumerator pullA() {
        float currentDuration = 0f;
        while (currentDuration < animationDuration * 1.05) {
            yield return null;
            trans.position = Vector3.SmoothDamp(trans.position, farPos, ref velocity, animationDuration - currentDuration);
            currentDuration += Time.deltaTime;
        }
    }

    private IEnumerator pullT() {
        float currentDuration = 0f;
        while (currentDuration < animationDuration * 1.05) {
            yield return null;
            trans.position = Vector3.SmoothDamp(trans.position, closePos, ref velocity, animationDuration - currentDuration);
            currentDuration += Time.deltaTime;
        }
    }
}
