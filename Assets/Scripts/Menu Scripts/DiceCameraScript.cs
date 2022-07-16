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
        pos = trans.position;
        closePos = pos;
        farPos = new Vector3(pos.x, pos.y, pos.z + farDistance);
    }

    public void pullAway() {
        StartCoroutine(pullA());
    }
   // public void pullToward() {
        //StartCoroutine();
    //}

    public IEnumerator pullA() {
        float currentDuration = 0f;
        Debug.Log(trans.gameObject);
        while (currentDuration < animationDuration) {
            yield return null;
            pos = Vector3.SmoothDamp(pos, farPos, ref velocity, animationDuration);
            currentDuration += Time.deltaTime;
        }
    }
}
