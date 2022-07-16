using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageBackground : MonoBehaviour
{
    [SerializeField] private float rate = 1f;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float magnitude;
    [SerializeField] private float height = 10;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(chipClock());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator chipClock() {
        while (true) {
            yield return new WaitForSeconds(rate);
            float magnitudeRand = magnitude + Random.Range(0, 15);
            float angle = Random.Range(0, 2 * Mathf.PI);
            Instantiate(prefab, new Vector3(magnitudeRand * Mathf.Sin(angle), height, magnitudeRand * Mathf.Cos(angle)), prefab.transform.rotation, GetComponent<Transform>());
        }
    }
}
