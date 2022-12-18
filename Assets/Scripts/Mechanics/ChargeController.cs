using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeController : Mechanic
{
    public List<Vector2Int> gatePos;
    public Vector3 gateDirection;

    PipFilterController pip;

    public int type, pips;

    public Material[] mats = new Material[2];

    public List<GameObject> doors;

    public bool pickedUp = false;
    public bool gateOpen = false;

    [SerializeField] private AudioClip pickupCharge, loseCharge;

    [SerializeField]
    private GameObject player;
    private DieController pScript;
    [SerializeField]
    private ManageGame mg;
    private MeshRenderer rend;
    private AudioSource source;
    private List<ChargeController> otherControllers = new List<ChargeController>(); //can easily be changed to be within game manager for optimization

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        source = GameObject.FindGameObjectWithTag("Audio").GetComponents<AudioSource>()[1];
        pScript = player.GetComponentInChildren<DieController>();
        mg = FindObjectOfType<ManageGame>();
        pip = GetComponentInChildren<PipFilterController>();
        rend = GetComponentInChildren<MeshRenderer>();
        rend.material = mats[0];

        pip.pips = pips;
        pip.player = player;

        foreach (List<GameObject> i in mg.chargeSwitchesInLevel) foreach (GameObject chargeSwitch in i)
        otherControllers.Add(chargeSwitch.GetComponentInChildren<ChargeController>());
        
    }
    public override void CheckForActivation()
    {
        if (!gateOpen)
        {
            // If the die is on the switch and the pip switch (if any) is activated give the corresponding charge.
            if (pip.MeetsPipRequirement(player) && pScript.position == position)
            {
                pickedUp = true;
                pScript.PowerDown(); // reset any existing charges before applying new ones
                pScript.PowerUp(type, Vector3Int.down);
                if (source is not null) {
                    source.PlayOneShot(pickupCharge, 1.0f);
                }
                rend.material = mats[1];
                pScript.currentCharge = this;
                pScript.chargeDirection = Vector3Int.down;

                foreach (ChargeController control in otherControllers) if (control != this && !control.gateOpen) {
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
                if (source is not null) {
                    source.PlayOneShot(loseCharge, 1.0f);
                }
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
