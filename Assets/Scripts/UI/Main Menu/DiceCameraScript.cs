using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DiceCameraScript : MonoBehaviour
{
    [SerializeField] private float farDistance = 20;
    [SerializeField] private float smoothDampTimeChange = 0.0005f;
    private Transform trans;
    private Vector3 pos;
    private Vector3 closePos;
    private Vector3 farPos;
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update

    void Awake() 
    {
        DebugManager.instance.enableRuntimeUI = false;
    }

    void Start()
    {
        trans = GetComponent<Transform>();
        closePos = trans.position;
        farPos = new Vector3(trans.position.x, trans.position.y, trans.position.z - farDistance);
    }

    public void pullDirection(float animationDuration, bool towards) {
        StartCoroutine(pull(animationDuration, towards));
    }
    

    private IEnumerator pull(float animationDuration, bool towards) {

        Vector3 target = towards ? closePos : farPos;
        float currentDuration = 0f;
        float animationTime = 0.2f;

        while (currentDuration < animationDuration) {
            yield return null;
            trans.position = Vector3.SmoothDamp(trans.position, target, ref velocity, animationTime);
            animationTime -= smoothDampTimeChange;
            currentDuration += Time.deltaTime;
        }
        trans.position = target;
    }

}
