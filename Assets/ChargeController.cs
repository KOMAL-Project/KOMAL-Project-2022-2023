using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeController : MonoBehaviour
{
    public Vector2Int pos;
    public List<Vector2Int> gatePos;
    public Vector3 gateDirection;

    public Material[] mats = new Material[2];

    public List<GameObject> doors;

    public bool pickedUp = false;
    public bool gateOpen = false;

    [SerializeField]
    private GameObject player;
    private DieController pScript;
    [SerializeField]
    private ManageGame mg;
    private MeshRenderer rend;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pScript = player.GetComponentInChildren<DieController>();
        mg = FindObjectOfType<ManageGame>();
        rend = GetComponentInChildren<MeshRenderer>();
        rend.material = mats[0];
    }

    void Update()
    {
        if (!gateOpen)
        {
            if (pScript.position == pos)
            {
                pickedUp = true;
                pScript.chargeDirection = Vector3.down;
                rend.material = mats[1];
            }
            if (pScript.chargeDirection == Vector3.zero)
            {
                pickedUp = false;
                rend.material = mats[0];
            }
            else 
            {
                for (int i = 0; i < gatePos.Count; i++)
                {
                    if (new Vector2Int(pScript.position.x + (int)pScript.chargeDirection.x, pScript.position.y + (int)pScript.chargeDirection.z) == gatePos[i])
                    {
                        gateOpen = true;
                        pickedUp = false;
                        rend.material = mats[1];

                        foreach (var door in doors) {
                            door.GetComponent<Animator>().SetBool("Active", false);
                        }

                        Debug.Log(doors.Count);

                        for (int j = 0; j < doors.Count; j++) 
                        {
                            mg.levelData[gatePos[j].x, gatePos[j].y] = null;
                        }
                    }
                }
            }
        }
    }
}
