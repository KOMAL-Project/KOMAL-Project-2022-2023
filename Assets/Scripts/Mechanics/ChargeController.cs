using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for Charge Givers.
/// <para> State 2: Charge giver is able to give charge and its charge is not on the die. This is default. </para>
/// <para> State 1: Charge giver cannot give charge and its charge is on the die. </para>
/// <para> State 0: Charge giver cannot give charge and doors of the same type have been used. </para>
/// </summary>
public class ChargeController : Mechanic
{
    public List<Vector2Int> gatePos;
    public Material[] mats = new Material[2];
    public List<GameObject> doors;
    private MeshRenderer rend;
    private List<ChargeController> otherControllers = new List<ChargeController>(); //can easily be changed to be within game manager for optimization

    private void Start()
    {
        rend = GetComponentInChildren<MeshRenderer>();
        rend.material = mats[0];

        foreach (List<GameObject> i in gameManager.chargeSwitchesInLevel) foreach (GameObject chargeSwitch in i)
        otherControllers.Add(chargeSwitch.GetComponentInChildren<ChargeController>());
        
    }
    //currently, things happen in this order: 1. all charges check if they attach their charge to the dice. 2. a charge checks for a reset, then checks for a matching door.
    public override void CheckForActivation() //this really only checks if the charge block and dice collide, not dice and door
    {
        if (state != 2)
        {
            // If the die is on the switch and the pip switch (if any) is activated give the corresponding charge.
            if (CheckPipFilter() && dieControl.position == position)
            {
                state = 1;
                dieControl.PowerDown(); // reset any existing charges before applying new ones
                dieControl.PowerUp(type, Vector3Int.down);
                rend.material = mats[1];
                dieControl.currentCharge = this;
                dieControl.chargeDirection = Vector3Int.down;

                foreach (ChargeController control in otherControllers) if (control != this && control.state != 2) {
                    control.state = 0;
                    control.rend.material = mats[0];
                }

            }
        }
    }

    public void UpdateChargeStatus()
    {
        if (state != 2)
        {
            // When charge face touches ground, reset charge.
            if (dieControl.chargeDirection == Vector3.zero && dieControl.currentCharge == this)
            {
                state = 0;
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
                            state = 2;
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
                                control.state = 2; 
                            }

                            break;
                        }
                    }
                }
            }
        }
    }

    public void activate() {

    }

    public override void setState(int input) {
        state = input;
        if (input == 2) {
            rend.material = mats[0];
            if (dieControl.currentCharge == this) {
                dieControl.PowerDown();
                dieControl.currentCharge = null;
            }

        }
        else if (input == 1) { //reset doors if they were down
            rend.material = mats[1];
            dieControl.currentCharge = this;
            dieControl.PowerUp(type, dieControl.chargeDirection);
            for (int j = 0; j < doors.Count; j++)
            {
                gameManager.levelData[gatePos[j].x, gatePos[j].y] = doors[j];
                doors[j].GetComponent<Animator>().SetBool("Active", true);
            }

        }
        else if (input == 0) { //charge connected
            dieControl.PowerDown();
            rend.material = mats[1];
            dieControl.currentCharge = null;
        }
        else Debug.Log("SOMETHING WENT WRONG");
    }




}
