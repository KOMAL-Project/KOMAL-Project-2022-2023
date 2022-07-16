using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyDice : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(Random.Range(0, 4), Random.Range(0, 4), Random.Range(0, 4)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        Destroy(gameObject);
    }
}
