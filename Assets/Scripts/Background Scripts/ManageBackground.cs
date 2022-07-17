using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageBackground : MonoBehaviour
{
    [SerializeField] private float rate = 1f;
    [SerializeField] private GameObject prefab;
    
    private Transform trans;

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
        StartCoroutine(chipClock());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator chipClock() {
        while (true) {
            yield return new WaitForSeconds(rate);
            Instantiate(prefab, new Vector3(Random.Range(-10, 10), Random.Range(-3, 7), Random.Range(5, 8)), Random.rotation, trans);
        }
    }
}
