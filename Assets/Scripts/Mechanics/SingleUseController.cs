using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUseController : Mechanic
{
    public GameObject player, manager;
    private DieController pDie;
    private bool primed, used;
    private Vector2Int playerPosition;


    // Start is called before the first frame update
    void Start()
    {
        primed = false;
        used = false;
        pDie = player.GetComponentInChildren<DieController>();
    }

    // Update is called once per frame

    public override void CheckForActivation() {

        playerPosition = pDie.position;

        if (playerPosition == position) primed = true;
        if (playerPosition != position && primed && !used)
        {
            manager.GetComponent<ManageGame>().levelData[position.x, position.y] = gameObject;
            used = true;
            GetComponentInChildren<Animator>().SetTrigger("Go");
        }
    }

    //0 is landed, 1 is primed, 20 is nothing (can easily change if this turns into multiple use tile)
    public byte GetStateByte() {
        if (!primed) return 20;
        else if (primed && !used) return 1;
        else return 0;
    }

    //undo stuff - could change maybe?
    public void ByteToSetState(byte input) {
        if (input == 20) {primed = used = false;}
        if (input == 1) {
            primed = true; used = false;
            GetComponentInChildren<Animator>().SetTrigger("Back");
            manager.GetComponent<ManageGame>().levelData[position.x, position.y] = null;
            
        }
        if (input == 0) {primed = used = true;}
    }
   
}
