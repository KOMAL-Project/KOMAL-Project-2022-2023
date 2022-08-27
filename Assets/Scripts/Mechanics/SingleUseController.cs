using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUseController : MonoBehaviour
{
    public GameObject player, manager;
    private DieController pDie;
    private bool primed, used;
    public Vector2Int position;
    private Vector2Int playerPosition;


    // Start is called before the first frame update
    void Start()
    {
        primed = false;
        used = false;
        pDie = player.GetComponentInChildren<DieController>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = pDie.position;

        if (playerPosition == position) primed = true;
        if (playerPosition != position && primed && !used)
        {
            manager.GetComponent<ManageGame>().levelData[position.x, position.y] = gameObject;
            used = true;
            GetComponentInChildren<Animator>().SetTrigger("Go");
        }
    }
}
