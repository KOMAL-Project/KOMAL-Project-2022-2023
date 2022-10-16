using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeController : MonoBehaviour
{
    public Vector2Int pos;
    public List<Vector2Int> gatePos;
    public Vector3 gateDirection;

    PipFilterController pip;

    public int type, pips;

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
        pip = GetComponentInChildren<PipFilterController>();
        rend = GetComponentInChildren<MeshRenderer>();
        rend.material = mats[0];

        pip.pips = pips;
        pip.thisPos = pos;
        pip.player = player;
        pip.playerPos = pScript.position;
    }

    void LateUpdate()
    {
        if (!gateOpen)
        {
            if (pip.MeetsPipRequirement(player) && pScript.position == pos)
            {
                if (pScript.chargeDirection != Vector3.zero && pScript.currentCharge != this) 
                {
                    if (pScript.currentCharge != null) 
                    {
                        pScript.currentCharge.pickedUp = false;
                        pScript.currentCharge.rend.material = mats[0];
                        pScript.currentCharge = null;
                    }
                    pScript.PowerDown();
                }
                Debug.Log("went over charge tile");

                pickedUp = true;
                pScript.PowerUp(type);
                pScript.chargeDirection = Vector3.down;
                rend.material = mats[1];
                pScript.currentCharge = this;
            }
            if (pScript.chargeDirection == Vector3.zero)
            {
                pickedUp = false;
                pScript.PowerDown();
                rend.material = mats[0];
                pScript.currentCharge = null;
            }
            else 
            {
                if (pScript.currentCharge == this)
                {
                    for (int i = 0; i < gatePos.Count; i++)
                    {
                        if (new Vector2Int(pScript.position.x + (int)pScript.chargeDirection.x, pScript.position.y + (int)pScript.chargeDirection.z) == gatePos[i])
                        {
                            gateOpen = true;
                            pickedUp = false;
                            pScript.PowerDown();
                            rend.material = mats[1];
                            pScript.currentCharge = null;

                            foreach (var door in doors)
                            {
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
}
