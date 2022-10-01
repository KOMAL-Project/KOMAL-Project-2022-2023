using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDice : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private float rate = 0.3f;
    [SerializeField] private float spawnDistance;
    [SerializeField] private float spawnWidth = 30;
    private Transform trans;
    private Vector3 pos;
    private float randX;
    

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
        pos = trans.position;
        StartCoroutine(DiceSpawn());
    }

    // Update is called once per frame
    
    private IEnumerator DiceSpawn() {
        while(true) {
            yield return new WaitForSeconds(rate);
            
            randX = 0 +  Random.Range(-spawnWidth, spawnWidth);
            Instantiate(prefabs[Random.Range(0,prefabs.Length)], new Vector3(pos.x + randX, pos.y + Random.Range(17.5f, 18.5f), pos.z + Random.Range(5, 11)), Random.rotation, trans);
        }
    }

    


}
