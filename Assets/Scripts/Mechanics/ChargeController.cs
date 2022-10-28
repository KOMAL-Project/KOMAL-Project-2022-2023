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
        
    }
    public void CheckForActivation()
    {
        if (!gateOpen)
        {
            if (pip.MeetsPipRequirement(player) && pScript.position == pos)
            {
                Debug.Log("went over charge tile " + type);
                if (pScript.chargeDirection != Vector3.zero && pScript.currentCharge != this) 
                {
                    if (pScript.currentCharge != null) 
                    {
                        pScript.currentCharge.pickedUp = false;
                        pScript.currentCharge.rend.material = mats[0];
                        pScript.currentCharge = null;
                    }
                    pScript.PowerDown();
                    rend.material = mats[0];
                    pScript.currentCharge = null;
                }

                pickedUp = true;
                pScript.PowerUp(type, Vector3.down);
                rend.material = mats[1];
                pScript.currentCharge = this;
                pScript.chargeDirection = Vector3.down;

            }
            if (pScript.chargeDirection == Vector3.zero && pScript.currentCharge != null)
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
                    Vector2Int chargePosition = new Vector2Int(pScript.position.x + (int)pScript.chargeDirection.x, pScript.position.y + (int)pScript.chargeDirection.z);
                    for (int i = 0; i < gatePos.Count; i++)
                    {
                        if (chargePosition == gatePos[i])
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

                            //Debug.Log(doors.Count);

                            for (int j = 0; j < doors.Count; j++)
                            {
                                mg.levelData[gatePos[j].x, gatePos[j].y] = null;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    public byte getStateByte() {
        if (!pickedUp && !gateOpen) return 2;
        else if (pickedUp && !gateOpen) return 1;
        else return 0;
    }

    public void ByteToSetState(byte input) {
        if (input == 2) {
            pickedUp = false;
            gateOpen = false;
            rend.material = mats[0];
            if (pScript.currentCharge == this) {
                pScript.PowerDown();
                pScript.currentCharge = null;
            }

        }
        else if (input == 1) { //reset doors if they were down
            gateOpen = false;
            pickedUp = true;
            rend.material = mats[1];
            pScript.currentCharge = this;
            pScript.PowerUp(type, pScript.chargeDirection);
            foreach (var door in doors) door.GetComponent<Animator>().SetBool("Active", true);
            for (int i = 0; i < doors.Count; i++)
            {
                mg.levelData[gatePos[i].x, gatePos[i].y] = doors[i];
            }

        }
        else if (input == 0) { //btw this shouldnt ever happen
            gateOpen = true;
            pScript.PowerDown();
            rend.material = mats[1];
            pScript.currentCharge = null;
        }
        else Debug.Log("SOMETHING WENT WRONG");
    }




}
