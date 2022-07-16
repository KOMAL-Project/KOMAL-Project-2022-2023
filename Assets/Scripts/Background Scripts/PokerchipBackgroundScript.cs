using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerchipBackgroundScript : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float rotationSpeed = 5;
    private Rigidbody rigid;
    
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.velocity = new Vector2(0, -speed);
        rigid.AddRelativeTorque(new Vector3(Random.Range(0, rotationSpeed), Random.Range(0, rotationSpeed), Random.Range(0, rotationSpeed)));

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other) {
        Destroy(gameObject);
    }
}
