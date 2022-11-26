using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


//record that contains all the necessary states info needed to revert move
public record states {
    //states of the die
    public Action ghostRotation;
    public Vector2Int mappedDieLocation;
    public Quaternion rotation;
    public Quaternion overlayRotation;
    //states of mechanics
    //charge controller states (if they exist) - 0 is no charge, 1 is charge on dice, 2 is charge used
    public bool? toggleState;
    public List<byte> limitedUseTileState; //0 is landed, 1 is primed, 20 is nothing (can easily change if this turns into multiple use tile)
    public List<byte> chargeState; //0 is activated, 1 is charge on dice, 2 is not being used
    public Vector3Int? chargeDirection;
    public List<byte> legoSwitchState; //0 is not active, 1 is active

    /// <summary>
    /// updates THIS states params to match OTHER states params while making OTHER state param null if they already matched.
    /// </summary>
    /// <param name="other"></param>
    public void updateStates(states other) {

        if (other.toggleState is not null && this.toggleState != other.toggleState) { //could I compact this?
            this.toggleState = other.toggleState;
        }
        else {
            //other.toggleState = null;
        }
        if (other.limitedUseTileState is not null && !(this.limitedUseTileState.SequenceEqual(other.limitedUseTileState))) {
            this.limitedUseTileState = other.limitedUseTileState;
            
        }
        else { //issue is that the state right before the change needs to be logged (which happens before anything even knows what the player is doing)
            //other.useTileState = null;
        }        
        if (other.chargeState is not null && !(this.chargeState.SequenceEqual(other.chargeState))) {
            this.chargeState = other.chargeState;   
        }
        else {
            //other.chargeState = null;
        }
        if (other.chargeDirection is not null && this.chargeDirection != other.chargeDirection) {
            this.chargeDirection = other.chargeDirection;
        }
        else {
            other.chargeDirection = null;
        }
        if (other.legoSwitchState is not null && !(this.legoSwitchState.SequenceEqual(other.legoSwitchState))) {
            this.legoSwitchState = other.legoSwitchState;
        }
        else {
            //other.legoSwitchState = null;
        }
    }

}

public class ActionRecorder : MonoBehaviour
{
    //big stacc
    Stack<states> stateStack;
    states currentState;

    //dice stuff
    public GameObject die;
    public DieController dieController;
    private Vector3 change = Vector3.zero;
    
    //mechanics stuff
    private ToggleSwitchController TSC; //toggle switch controller
    private List<SingleUseController> SUC; //single use controller
    private List<ChargeController> CC; //charge controllers
    private List<LegoSwitchController> LSC; //Lego switch controller


    // Start is called before the first frame update
    void Start()
    {
        stateStack = new Stack<states>();

        dieController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        die = dieController.transform.gameObject;

        //sets the change vector for MapToActualPosition()
        Vector3 actual = die.transform.position;
        Vector2Int mapped = dieController.position;
        change = new Vector3(actual.x - mapped.x, actual.y, actual.z - mapped.y);

        ManageGame gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<ManageGame>();

        if (gameManager.toggleSwitchesInLevel.Count > 0)
        TSC = gameManager.toggleSwitchesInLevel[0].GetComponentInChildren<ToggleSwitchController>();

        SUC = new List<SingleUseController>(){};
        foreach (GameObject t in gameManager.singleUseTilesInLevel) SUC.Add(t.GetComponent<SingleUseController>());

        CC = new List<ChargeController>();
        foreach(List<GameObject> l in gameManager.chargeSwitchesInLevel) foreach(GameObject g in l) CC.Add(g.GetComponent<ChargeController>());

        LSC = new List<LegoSwitchController>();
        foreach(List<GameObject> l in gameManager.legoSwitchesInLevel) foreach(GameObject g in l) LSC.Add(g.GetComponent<LegoSwitchController>());


        currentState = getState();
        stateStack.Push(getState());


    }

/// <summary>
/// Gets the current state when called
/// </summary>
/// <returns></returns>
    public states getState() {

        List<byte> SingleUseStates = new List<byte>();
        foreach (SingleUseController t in SUC) SingleUseStates.Add(t.GetStateByte());
        List<byte> ChargeStates = new List<byte>();
        foreach (ChargeController t in CC) ChargeStates.Add(t.getStateByte());
        List<byte> LegoStates = new List<byte>();
        foreach (LegoSwitchController t in LSC) LegoStates.Add(t.getStateByte());
        Debug.Log(string.Join(", ", LegoStates));

        return new states {
            ghostRotation = dieController.lastAction,
            mappedDieLocation = dieController.position,
            rotation = die.transform.rotation,
            overlayRotation = dieController.doc.overlayDie.transform.rotation,
            toggleState = (TSC is not null ? TSC.stateToGetBool() : null),
            limitedUseTileState = (SUC.Count != 0 ? SingleUseStates: null),
            chargeState = (CC.Count != 0 ? ChargeStates: null),
            chargeDirection = dieController.chargeDirection,
            legoSwitchState = (LSC.Count != 0? LegoStates: null)
        };
    }

    /// <summary>
    /// Stores the last state and puts it in the stack 
    /// </summary>
    public void Record() {
        
        states newState = getState();
        currentState.updateStates(newState);
        stateStack.Push(newState);
        //Debug.Log(currentState);
    }
    /// <summary>
    /// Returns to the last set of states
    /// </summary>
    public void Undo() {

        if (stateStack.Count <= 1) {
            return;
        }

        //gets the state that the dice just moved too (obtained right after the move happened)
        states mechanicsState = stateStack.Pop();

        //gets the state that it is returning to (obtained right before the move happened)
        states moveState = stateStack.Peek();

        //mechanics
        if (moveState.toggleState is not null) TSC.boolToSetState((bool)moveState.toggleState);
        //TSC.CheckForActivation(); //could change this later if trying to change it to snap instead of activating

        if (moveState.limitedUseTileState is not null) for (int i = 0; i < SUC.Count; i++) SUC[i].ByteToSetState(moveState.limitedUseTileState[i]);

        if (mechanicsState.chargeDirection is not null) dieController.chargeDirection = (Vector3Int)mechanicsState.chargeDirection;
        if (moveState.chargeState is not null) for (int i = 0; i < CC.Count; i++) CC[i].ByteToSetState(moveState.chargeState[i]);

        if (moveState.legoSwitchState is not null) for (int i = 0; i < LSC.Count; i++) LSC[i].ByteToSetState(moveState.legoSwitchState[i]);

        ReverseTurn(mechanicsState.ghostRotation)(); //note that this MUST happen before the position is moved since mechanics rely on last position

        //dice
        dieController.position = moveState.mappedDieLocation;
        die.transform.position = MapToActualPosition(moveState.mappedDieLocation);
        die.transform.rotation = moveState.rotation;
        dieController.doc.overlayDie.transform.rotation = moveState.overlayRotation;

        currentState.updateStates(moveState);
    }

    /// <summary>
    /// Takes a turn direction (ghosts and charge) and does the opposite
    /// </summary>
    /// <param name="input"</param> 
    /// <returns></returns>
    public Action ReverseTurn(Action input) {

        List<Action> moves = new List<Action> { dieController.MoveForward, dieController.MoveLeft, dieController.MoveBack, dieController.MoveRight };
        int index = moves.IndexOf(input);
        return moves[(index + 2) % 4];

    }
/// <summary>
/// Converts a mapped vector2 to a actual position in space vector3
/// </summary>
/// <param name="mapped"></param>
/// <returns></returns>
    public Vector3 MapToActualPosition(Vector2Int mapped) {
        return new Vector3(change.x + mapped.x, change.y, change.z + mapped.y);
    }




}
