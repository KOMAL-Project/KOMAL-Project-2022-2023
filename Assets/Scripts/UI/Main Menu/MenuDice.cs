using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDice : MonoBehaviour
{
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private Material[] materials;
    [SerializeField] private float rate = 0.3f;
    [SerializeField] private float spawnDistance;
    [SerializeField] private float spawnWidth = -1;
    [SerializeField] private GameObject fallingPrefab;
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
    
    private IEnumerator DiceSpawn() {
        while(true) {
            yield return new WaitForSeconds(rate);

            if (spawnWidth == -1) {
                spawnWidth = MainMenuScript.Xoffset / 108;
            }
            
            randX = 0 +  Random.Range(-spawnWidth, spawnWidth);
            int randomMesh = Random.Range(0,meshes.Length);
            GameObject obj = Instantiate(fallingPrefab, new Vector3(pos.x + randX, pos.y + Random.Range(18.5f, 19.5f), pos.z + Random.Range(5, 11)), Random.rotation, trans);
            obj.GetComponent<MeshRenderer>().material = materials[randomMesh];
            obj.GetComponent<MeshFilter>().mesh = meshes[randomMesh];
        }
    }

    


}
