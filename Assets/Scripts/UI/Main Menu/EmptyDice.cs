using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyDice : MonoBehaviour
{
    [SerializeField] float minAppliedTorque;
    [SerializeField] float maxAppliedTorque;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(Random.Range(minAppliedTorque, maxAppliedTorque), Random.Range(minAppliedTorque, maxAppliedTorque), Random.Range(minAppliedTorque, maxAppliedTorque)));
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other) {
        Destroy(gameObject);
    }
}
