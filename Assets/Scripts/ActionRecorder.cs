using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//record that contains all the necessary states info needed to revert move
public record states {
    //states of the die
    public Action ghostRotation;
    public Vector2Int mappedDieLocation;
    public Quaternion rotation;
    //charge controller states (if they exist) - 0 is no charge, 1 is charge on dice, 2 is charge used
    public bool? toggleState;
    public List<byte?> useTileState; //0 is landed, 1 is primed, 20 is nothing (can easily change if this turns into multiple use tile)

    //updates THIS states params to match OTHER states params while making OTHER state param null if they already matched.
    public void updateStates(states other) {

        if (this.toggleState != other.toggleState && other.toggleState is not null) { //could I compact this?
            this.toggleState = other.toggleState;
        }
        else {
            other.toggleState = null;
        }

        if (this.useTileState != other.useTileState && other.useTileState is not null) {
            this.useTileState = other.useTileState;
        }
        else {
            other.useTileState = null;
        }


    }

    public override string ToString() {
        return(""+toggleState);
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
    //public Dictionary<string, List<GameObject>> mechanicObjects;
    private ToggleSwitchController TSC;
    private List<SingleUseController> SUC;


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
        
        TSC = gameManager.toggleSwitchesInLevel[0].GetComponentInChildren<ToggleSwitchController>();
        SUC = new List<SingleUseController>(){};
        Debug.Log(gameManager.singleUseTilesInLevel);
        foreach (GameObject t in gameManager.singleUseTilesInLevel) SUC.Add(t.GetComponent<SingleUseController>());

        currentState = getState();
        stateStack.Push(currentState);


    }

    private void Update() {
        //could be added somewhere else
        if (Input.GetKeyUp("k")) {
            Undo();
        }
    }

//gets the most current state with all info
    public states getState() {

        List<byte?> SingleTileStates = new List<byte?>();
        foreach (SingleUseController t in SUC) SingleTileStates.Add(t.GetStateByte());

        return new states {
            ghostRotation = dieController.lastAction,
            mappedDieLocation = dieController.position,
            rotation = die.transform.rotation,
            toggleState = (TSC is not null ? TSC.stateToGetBool() : null),
            useTileState = (SUC is not null ? SingleTileStates: null)
        };
    }

    //stores the last state and puts it in the stack    
    public void Record() {
        
        states newState = getState();
        currentState.updateStates(newState);
        stateStack.Push(newState);
        //Debug.Log(currentState);

    }

    public void Undo() {

        if (stateStack.Count <= 1) {
            return;
        }

        //gets the state that the dice just moved too (obtained right after the move happened)
        states mechanicsState = stateStack.Pop();

        //gets the state that it is returning to (obtained right before the move happened)
        states moveState = stateStack.Peek();

        //mechanics (the ?? are there to work with bool? and byte?)
        if (mechanicsState.toggleState is not null) TSC.boolToSetState(mechanicsState.toggleState ?? false);
        TSC.CheckForActivation(); //could change this later so that TSC snaps instead of animates
        if (moveState.useTileState is not null) for (int i = 0; i < SUC.Count; i++) SUC[i].ByteToSetState(moveState.useTileState[i] ?? 50);

        ReverseTurn(moveState.ghostRotation)(); //note that this MUST happen before the position is moved since mechanics rely on last position

        //dice
        dieController.position = moveState.mappedDieLocation;
        die.transform.position = MapToActualPosition(moveState.mappedDieLocation);
        die.transform.rotation = moveState.rotation;

        currentState.updateStates(moveState);
        

    }

    //takes a turn direction (ghosts, charge) and does the opposite
    public Action ReverseTurn(Action input) {

        List<Action> moves = new List<Action> { dieController.MoveForward, dieController.MoveLeft, dieController.MoveBack, dieController.MoveRight };
        int index = moves.IndexOf(input);
        return moves[(index + 2) % 4];

    }

    //converts a mapped vector2 to a actual position in space vector3
    public Vector3 MapToActualPosition(Vector2Int mapped) {

        return new Vector3(change.x + mapped.x, change.y, change.z + mapped.y);
    }




}
