using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeController : MonoBehaviour
{
    public Vector2Int pos;
    public Vector2Int gatePos;
    public Vector3 gateDirection;

    public List<GameObject> doors;

    public bool pickedUp = false;
    public bool gateOpen = false;

    [SerializeField]
    private GameObject player;
    private DieController pScript;
    [SerializeField]
    private ManageGame mg;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pScript = player.GetComponent<DieController>();
        mg = FindObjectOfType<ManageGame>();
    }

    void Update()
    {
        if (!gateOpen)
        {
            if (pScript.position == pos)
            {
                pickedUp = true;
                pScript.chargeDirection = Vector3.down;
            }
            if (pScript.chargeDirection == Vector3.zero)
            {
                pickedUp = false;
            }
            else if (new Vector2Int(pScript.position.x + (int)pScript.chargeDirection.x, pScript.position.y + (int)pScript.chargeDirection.z) == gatePos)
            {
                gateOpen = true;
                pickedUp = false;
                foreach (GameObject door in doors) {
                    Destroy(door);
                }
            }
        }
    }
}
