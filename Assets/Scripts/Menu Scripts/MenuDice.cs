using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDice : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float rate = 0.3f;
    [SerializeField] private float spawnDistance;
    [SerializeField] private float spawnWidth;
    private Transform trans;
    private Vector3 pos;
    private bool close;
    private float initialSpawnWidth;
    private int height = 4;
    

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
        pos = trans.position;
        close = true;
        initialSpawnWidth = spawnWidth;
        StartCoroutine(DiceSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator DiceSpawn() {
        while(true) {
            yield return new WaitForSeconds(rate);
            float randX = Random.Range(1, 3) == 1 ? spawnDistance : -spawnDistance;
            if (!close) {
                randX = 0;
            }
            
            randX += Random.Range(-spawnWidth, spawnWidth);
            Instantiate(prefab, new Vector3(pos.x + randX, pos.y + height, pos.z + 5), Random.rotation, trans);
        }
    }

    public void toggleDiceDistance() {
        if (close) {
            spawnWidth = 30;
            close = false;
            height = 15;
        }
        else {
            spawnWidth = initialSpawnWidth;
            close = true;
            height = 4;
        }
    }
    


}
