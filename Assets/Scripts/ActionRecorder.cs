using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


//record that contains all the necessary states info needed to revert move
public struct States {
    //states of the die
    public Action ghostRotation; // the function that handles rotation of the die's face structure
    public Vector2Int mappedDieLocation; // tile position of the die on the board
    public Quaternion rotation; // rotation of the die
    public Dictionary<Vector3Int, int> sides; // contains which pip (or charge type) is on which direction of the die
    public Quaternion overlayRotation; // rotation of the overlay
    public Vector3 dieChargeMeshLocalEulers; // the Local Euler Angles of the mesh for charges attached to the die
    //states of mechanics
    //charge controller states (if they exist) - 0 is no charge, 1 is charge on dice, 2 is charge used
    public int? toggleState;
    public List<int> limitedUseTileState; //0 is landed, 1 is primed, 20 is nothing (can easily change if this turns into multiple use tile)
    public List<int> chargeState; //0 is activated, 1 is charge on dice, 2 is not being used
    public Vector3Int? chargeDirection;
    public List<int> legoSwitchState; //0 is not active, 1 is active

    /// <summary>
    /// Matches the existing state's contents to those of another state.
    /// updates THIS states params to match OTHER states params while making OTHER state param null if they already matched.
    /// </summary>
    /// <param name="other"></param>
    public void UpdateStates(States other) 
    {

        if (other.toggleState is not null && this.toggleState != other.toggleState) 
        { //could I compact this?
            this.toggleState = other.toggleState;
        }
        else 
        {
            //other.toggleState = null;
        }
        if (other.limitedUseTileState is not null && !(this.limitedUseTileState.SequenceEqual(other.limitedUseTileState))) 
        {
            this.limitedUseTileState = other.limitedUseTileState;
            
        }
        else 
        { //issue is that the state right before the change needs to be logged (which happens before anything even knows what the player is doing)
            //other.useTileState = null;
        }        
        if (other.chargeState is not null && !(this.chargeState.SequenceEqual(other.chargeState))) {
            this.chargeState = other.chargeState;   
        }
        else 
        {
            //other.chargeState = null;
        }
        if (other.chargeDirection is not null && this.chargeDirection != other.chargeDirection) {
            this.chargeDirection = other.chargeDirection;
        }
        else 
        {
            //other.chargeDirection = null;
        }
        if (other.legoSwitchState is not null && !(this.legoSwitchState.SequenceEqual(other.legoSwitchState))) {
            this.legoSwitchState = other.legoSwitchState;
        }
        else 
        {
            //other.legoSwitchState = null;
        }
    }

}

public class ActionRecorder : MonoBehaviour
{
    //big stacc
    Stack<States> stateStack;
    States currentState;

    //dice stuff
    public GameObject die, overlayDie;
    public DieController dieController;
    public DieOverlayController doc;
    private Vector3 change = Vector3.zero;
    
    //mechanics stuff
    private ToggleSwitchController TSC; //toggle switch controller
    private List<SingleUseController> SUC; //single use controller
    private List<ChargeController> CC; //charge controllers
    private List<LegoSwitchController> LSC; //Lego switch controller

    private AudioSourceManager source;
    void Start()
    {
        stateStack = new Stack<States>();

        dieController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        die = dieController.transform.gameObject;
        overlayDie = GameObject.FindGameObjectWithTag("OverlayDie");
        doc = dieController.doc;
        source = AudioSourceManager.Instance;

        //sets the change vector for MapToActualPosition()
        Vector3 actual = die.transform.position;
        Vector2Int mapped = dieController.position;
        change = new Vector3(actual.x - mapped.x, actual.y, actual.z - mapped.y);

        ManageGame gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<ManageGame>();

        // Since toggle switch controllers all know each other we only need to update one to update the rest
        if (gameManager.toggleSwitchControllers.Count > 0)
        TSC = gameManager.toggleSwitchControllers[0];

        // Create lists of all of the other mechanics components
        SUC = new List<SingleUseController>(){};
        foreach (GameObject t in gameManager.singleUseTilesInLevel) SUC.Add(t.GetComponent<SingleUseController>());

        CC = new List<ChargeController>();
        foreach(List<ChargeController> l in gameManager.chargeControllers) foreach(ChargeController g in l) CC.Add(g);

        LSC = new List<LegoSwitchController>();
        foreach(List<LegoSwitchController> l in gameManager.legoSwitchControllers) foreach(LegoSwitchController g in l) LSC.Add(g);

        // push the initial board state to the stack
        currentState = GetState();
        stateStack.Push(GetState());


    }
    /// <summary>
    /// Returns a record containing the states of every part of the game (mechanic states, die + charge orientation, etc.)
    /// </summary>
    /// <returns></returns>
    public States GetState() 
    {
        //Debug.Log("get state" + dieController.chargeDirection);
        List<int> SingleUseStates = new List<int>();
        foreach (SingleUseController t in SUC) SingleUseStates.Add(t.getState());
        List<int> ChargeStates = new List<int>();
        foreach (ChargeController t in CC) ChargeStates.Add(t.getState());
        List<int> LegoStates = new List<int>();
        foreach (LegoSwitchController t in LSC) LegoStates.Add(t.getState());

        return new States
        {
            ghostRotation = dieController.dieRollSideFunction,
            mappedDieLocation = dieController.position,
            rotation = die.transform.rotation,
            sides = dieController.sides,
            dieChargeMeshLocalEulers = dieController.chargeFaceObject.transform.localEulerAngles,
            overlayRotation = dieController.doc.overlayDie.transform.rotation,
            // Mechanics -- we only add them to the list if they exist, otherwise we return null.
            toggleState = (TSC is not null ? TSC.getState() : null),
            limitedUseTileState = (SUC.Count != 0 ? SingleUseStates: null),
            chargeState = (CC.Count != 0 ? ChargeStates: null),
            chargeDirection = dieController.chargeDirection,
            legoSwitchState = (LSC.Count != 0 ? LegoStates: null)
        };
    }

    /// <summary>
    /// Uses updateStates to match the contents of currentState to that of the board's current state "newState", 
    /// then pushes newState to the stack.
    /// </summary>
    public void Record() 
    {
        States newState = GetState();
        currentState.UpdateStates(newState);
        stateStack.Push(newState);
        //Debug.Log(currentState);
    }
    /// <summary>
    /// Returns to the last set of states
    /// </summary>
    public void Undo() 
    {
        // Prevents function execution if we haven't moved yet and there is nothing to undo to.
        if (stateStack.Count <= 1) return;

        // clearing the current state away from the stack
        States presentState = stateStack.Pop();

        // get the state one step back in time
        States oneStepBackState = stateStack.Peek();

        // Die Rotation reversal
        ReverseTurn(presentState.ghostRotation)();
        if (oneStepBackState.chargeDirection is not null) dieController.chargeDirection = (Vector3Int)oneStepBackState.chargeDirection;

        // Die Position Undo -- this NEEDS to be before the mechanics are undone
        die.transform.position = MapToActualPosition(oneStepBackState.mappedDieLocation);
        dieController.position = oneStepBackState.mappedDieLocation;
        die.transform.rotation = oneStepBackState.rotation;
        dieController.sides = oneStepBackState.sides;
        dieController.chargeFaceObject.transform.localEulerAngles = oneStepBackState.dieChargeMeshLocalEulers;
        // we do this already in DieOverlay controller but need to do it here 
        // to make sure the overlay updates in time to recieve a Charge on the correct side.
        overlayDie.transform.localEulerAngles = die.transform.eulerAngles + new Vector3(0, 180 - 45, 0);
        //doc.chargeFaceObj.transform.localEulerAngles = oneStepBackState.dieChargeMeshLocalEulers;


        // set the states of mechanics to that of moveState
        if (oneStepBackState.toggleState is not null) TSC.SetState((int)oneStepBackState.toggleState);
        if (oneStepBackState.limitedUseTileState is not null) for (int i = 0; i < SUC.Count; i++) SUC[i].SetState(oneStepBackState.limitedUseTileState[i]);
        if (oneStepBackState.legoSwitchState is not null) for (int i = 0; i < LSC.Count; i++) LSC[i].SetState(oneStepBackState.legoSwitchState[i]);
        if (oneStepBackState.chargeState is not null) for (int i = 0; i < CC.Count; i++) CC[i].SetState(oneStepBackState.chargeState[i]);

        // And now set our current state to the state one step back in time to complete the undo.
        // Since the oneStepBack state is still in the stack we don't need to call Record()
        currentState.UpdateStates(oneStepBackState);

        source.playSound("Undo", 1);
    }

    /// <summary>
    /// Takes in a Die Move function (MoveRight, MoveLeft, MoveBack, etc.) and return's its opposite.
    /// </summary>
    /// <param name="input"</param> 
    /// <returns></returns>
    public Action ReverseTurn(Action input) 
    {
        List<Action> moves = new List<Action> { dieController.MoveForward, dieController.MoveLeft, dieController.MoveBack, dieController.MoveRight };
        int index = moves.IndexOf(input);
        return moves[(index + 2) % 4];
    }


    /// <summary>
    /// Converts a mapped vector2 to a actual position in space vector3
    /// </summary>
    /// <param name="mapped"></param>
    /// <returns></returns>
    public Vector3 MapToActualPosition(Vector2Int mapped) 
    {
        return new Vector3(change.x + mapped.x, change.y, change.z + mapped.y);
    }




}
