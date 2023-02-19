using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

/// <summary>
/// Script for Charge Givers.
/// <para> State 0: Charge giver is able to give charge and its charge is not on the die. This is default. </para>
/// <para> State 1: Charge giver cannot give charge and its charge is on the die. </para>
/// <para> State 2: Charge giver cannot give charge and gates of the same type have been used. </para>
/// </summary>
public class ChargeController : Mechanic
{
    public List<ChargeController>[] controllers;
    public List<GameObject> gates;
    public List<Vector2Int> gatePos;
    [SerializeField] private AudioClip pickupCharge, loseCharge;
    public Material[] mats = new Material[2];
    private MeshRenderer rend;
    private AudioSource source;
    private void Start()
    {
        
        rend = GetComponentInChildren<MeshRenderer>();
        source = MusicManager.Instance.GetComponents<AudioSource>()[1];
        rend.material = mats[0];
        state = 0;
        
    }
    //currently, things happen in this order: 1. all charges check if they attach their charge to the dice. 2. a charge checks for a reset, then checks for a matching door.
    public override void CheckForActivation() //this ONLY checks if the charge block and dice collide, not dice and door
    {
        if (state != 2 && CheckPipFilter() && dieControl.position == position)
        {
            // Activate this switch
            activateSelf(Vector3.down);

            //forces all other charges to go back to normal
            foreach (List<ChargeController> _controllers in controllers) foreach (ChargeController control in _controllers) 
            if (control != this && control.state != 2 && control.type == this.type) {
                control.state = 0;
                control.rend.material = mats[0];
                control.pipFilter.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = control.pipFilter.pips > 0;
            }
        }
    }

    // Checks for charge touching cards and charge touching ground
    public void UpdateChargeStatus()
    {
        if (state != 2)
        {
            // When charge face touches ground, reset charge.
            if (dieControl.chargeDirection == Vector3.down && dieControl.currentCharge == this && this.position != dieControl.position)
            {
                deactivateSelf(false);
                pipFilter.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = pipFilter.pips > 0;
                if (source is not null) {
                    source.PlayOneShot(loseCharge, 1.0f);
                }

            }
            else if (dieControl.currentCharge == this)
            {
                Vector2Int chargePosition = new Vector2Int(dieControl.position.x + (int)dieControl.chargeDirection.x, dieControl.position.y + (int)dieControl.chargeDirection.z);
                for (int i = 0; i < gatePos.Count; i++) //runs through each door to see if its the same position as the charge
                {
                    if (chargePosition == gatePos[i])
                    {
                        deactivateSelf(true);

                        setGates(false); //changes all the gates to be down

                        foreach (ChargeController control in controllers[type]) {
                            control.rend.material = mats[1];
                            control.state = 2;
                            control.pipFilter.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
                        }

                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// causes the charge to be placed onto the dice. direction is the direction that the charge is placed (most often down).
    /// </summary>
    /// <param name="direction"></param>
    public void activateSelf(Vector3 direction) {
        state = 1;
        dieControl.PowerDown(); // reset any existing charges before applying new ones
        dieControl.PowerUp(type, Vector3Int.down);
        rend.material = mats[1];
        dieControl.currentCharge = this;
        dieControl.chargeDirection = Vector3Int.down;
        pipFilter.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// causes the charge to be removed from the die. used is a parameter for whether the charge is used (state 2) or destroyed (state 0).
    /// i wouldnt blame you if you changed this variable name
    /// </summary>
    /// <param name="active"></param>
    public void deactivateSelf(bool used) {
        dieControl.PowerDown();
        dieControl.currentCharge = null;

        if (used) { //unable to be used afterwards
            state = 2;
            rend.material = mats[1];
            
        } else { //still able to be used afterwards
            state = 0;
            rend.material = mats[0];
            dieControl.chargeDirection = Vector3Int.zero;
            pipFilter.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = pipFilter.pips > 0;
        }
    }

    /// <summary>
    /// sets the gates to a certain state. active is blocking and taking up space.
    /// </summary>
    /// <param name="active"></param>
    public void setGates(bool active) {
        if (active) {
            for (int j = 0; j < gates.Count; j++)
            {
                gameManager.levelData[gatePos[j].x, gatePos[j].y] = gates[j];
                gates[j].GetComponent<Animator>().SetBool("Active", true);
            }
        } else {
            for (int j = 0; j < gates.Count; j++)
            {
                gameManager.levelData[gatePos[j].x, gatePos[j].y] = null;
                gates[j].GetComponent<Animator>().SetBool("Active", false);
            }
        }
    }

    public override void setState(int input) {
        if (input == 0 & state != 0) {
            rend.material = mats[0];
            if (dieControl.currentCharge == this) {
                dieControl.PowerDown();
                dieControl.currentCharge = null;
            }

        }
        else if (input == 1 & state != 1) {
            rend.material = mats[1];
            dieControl.currentCharge = this;
            if (state == 2) {
                dieControl.PowerUp(type, Vector3Int.up);
                setGates(true);
            } else {
                dieControl.PowerUp(type, Vector3Int.down);
            }

        }
        else if (input == 2 & state != 2) { //charge connected
            dieControl.PowerDown();
            rend.material = mats[1];
            dieControl.currentCharge = null;
        }
        state = input;
    }




}
