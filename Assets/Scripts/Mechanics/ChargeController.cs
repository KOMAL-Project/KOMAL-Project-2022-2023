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
    private List<ChargeController> otherControllers = new List<ChargeController>();

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

        foreach (List<GameObject> i in mg.chargeSwitchesInLevel) foreach (GameObject chargeSwitch in i)
        otherControllers.Add(chargeSwitch.GetComponentInChildren<ChargeController>());
        
    }
    public void CheckForActivation()
    {
        if (!gateOpen)
        {
            // If the die is on the switch and the pip switch (if any) is activated give the corresponding charge.
            if (pip.MeetsPipRequirement(player) && pScript.position == pos)
            {
                pickedUp = true;
                pScript.PowerUp(type, Vector3Int.down);
                rend.material = mats[1];
                pScript.currentCharge = this;
                pScript.chargeDirection = Vector3Int.down;

                foreach (ChargeController control in otherControllers) if (control != this){
                    control.pickedUp = false;
                    control.rend.material = mats[0];
                }

            }
        }
    }

    public void UpdateChargeStatus()
    {
        if (!gateOpen)
        {
            // When charge face touches ground, reset charge.
            if (pScript.chargeDirection == Vector3.zero && pScript.currentCharge == this)
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
                            pScript.currentCharge = null;

                            for (int j = 0; j < doors.Count; j++)
                            {
                                mg.levelData[gatePos[j].x, gatePos[j].y] = null;
                                doors[j].GetComponent<Animator>().SetBool("Active", false);
                            }
                            foreach (GameObject obj in mg.chargeSwitchesInLevel[type]) {
                                ChargeController control = obj.GetComponentInChildren<ChargeController>();
                                control.rend.material = mats[1];
                                control.gateOpen = true; 
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
            for (int j = 0; j < doors.Count; j++)
            {
                mg.levelData[gatePos[j].x, gatePos[j].y] = doors[j];
                doors[j].GetComponent<Animator>().SetBool("Active", true);
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
