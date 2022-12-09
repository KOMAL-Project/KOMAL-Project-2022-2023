using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeController : Mechanic
{
    public List<Vector2Int> gatePos;
    public Vector3 gateDirection;
    public Material[] mats = new Material[2];

    public List<GameObject> doors;

    public bool pickedUp = false;
    public bool gateOpen = false;
    private MeshRenderer rend;
    private List<ChargeController> otherControllers = new List<ChargeController>(); //can easily be changed to be within game manager for optimization

    private void Start()
    {
        rend = GetComponentInChildren<MeshRenderer>();
        rend.material = mats[0];

        foreach (List<GameObject> i in gameManager.chargeSwitchesInLevel) foreach (GameObject chargeSwitch in i)
        otherControllers.Add(chargeSwitch.GetComponentInChildren<ChargeController>());
        
    }
    public override void CheckForActivation()
    {
        if (!gateOpen)
        {
            // If the die is on the switch and the pip switch (if any) is activated give the corresponding charge.
            if (CheckPipFilter() && dieControl.position == position)
            {
                pickedUp = true;
                dieControl.PowerDown(); // reset any existing charges before applying new ones
                dieControl.PowerUp(type, Vector3Int.down);
                rend.material = mats[1];
                dieControl.currentCharge = this;
                dieControl.chargeDirection = Vector3Int.down;

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
            if (dieControl.chargeDirection == Vector3.zero && dieControl.currentCharge == this)
            {
                pickedUp = false;
                dieControl.PowerDown();
                rend.material = mats[0];
                dieControl.currentCharge = null;
            }
            else
            {
                if (dieControl.currentCharge == this)
                {
                    Vector2Int chargePosition = new Vector2Int(dieControl.position.x + (int)dieControl.chargeDirection.x, dieControl.position.y + (int)dieControl.chargeDirection.z);
                    for (int i = 0; i < gatePos.Count; i++)
                    {
                        if (chargePosition == gatePos[i])
                        {
                            gateOpen = true;
                            pickedUp = false;
                            dieControl.PowerDown();
                            dieControl.currentCharge = null;

                            for (int j = 0; j < doors.Count; j++)
                            {
                                gameManager.levelData[gatePos[j].x, gatePos[j].y] = null;
                                doors[j].GetComponent<Animator>().SetBool("Active", false);
                            }
                            foreach (GameObject obj in gameManager.chargeSwitchesInLevel[type]) {
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
            if (dieControl.currentCharge == this) {
                dieControl.PowerDown();
                dieControl.currentCharge = null;
            }

        }
        else if (input == 1) { //reset doors if they were down
            gateOpen = false;
            pickedUp = true;
            rend.material = mats[1];
            dieControl.currentCharge = this;
            dieControl.PowerUp(type, dieControl.chargeDirection);
            for (int j = 0; j < doors.Count; j++)
            {
                gameManager.levelData[gatePos[j].x, gatePos[j].y] = doors[j];
                doors[j].GetComponent<Animator>().SetBool("Active", true);
            }

        }
        else if (input == 0) { //btw this shouldnt ever happen
            gateOpen = true;
            dieControl.PowerDown();
            rend.material = mats[1];
            dieControl.currentCharge = null;
        }
        else Debug.Log("SOMETHING WENT WRONG");
    }




}
