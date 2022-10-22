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
    public List<bool?> useTileState; //what is this going to be called now
    public bool? toggleState;
    public List<byte?> chargeStates; 

}

public class ActionRecorder : MonoBehaviour
{
    //big stacc
    Stack<states> stateStack;

    //dice stuff
    public GameObject die;
    public DieController dieController;
    private Vector3 change = Vector3.zero;
    
    //mechanics stuff
    public Dictionary<string, List<GameObject>> mechanicObjects;


    // Start is called before the first frame update
    void Start()
    {
        stateStack = new Stack<states>();

        dieController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        die = dieController.transform.gameObject;

        ManageGame gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<ManageGame>();
        mechanicObjects = new Dictionary<string, List<GameObject>>() {
            {"toggleSwitches", gameManager.toggleSwitchesInLevel},

            };


    }

    private void Update() {
        //could be added somewhere else
        if (Input.GetKeyUp("k")) {
            Undo();
        }
    }

    //stores the last state and puts it in the stack    
    public void Record() {
        
        //gets the last state to compare (For changes)
        states oldStack = stateStack.Count > 0 ? stateStack.Peek() : new states{};
        
        stateStack.Push(new states {
            ghostRotation = dieController.lastAction,
            mappedDieLocation = dieController.position,
            rotation = die.transform.rotation,
            toggleState = null
        });

    }

    public void Undo() {

        if (stateStack.Count == 0) {
            return;
        }         

        //gets the state that it is returning to
        states undoState = stateStack.Pop();

        // does stuff with it
        //dice
        ReverseTurn(undoState.ghostRotation)();
        dieController.position = undoState.mappedDieLocation;
        die.transform.position = MapToActualPosition(undoState.mappedDieLocation);
        die.transform.rotation = undoState.rotation;

        //charges
    
    }

    //takes a turn direction (ghosts, charge) and does the opposite
    public Action ReverseTurn(Action input) {

        List<Action> moves = new List<Action> { dieController.MoveForward, dieController.MoveLeft, dieController.MoveBack, dieController.MoveRight };
        int index = moves.IndexOf(input);
        return moves[(index + 2) % 4];

    }

    //converts a mapped vector2 to a actual position in space vector3
    public Vector3 MapToActualPosition(Vector2Int mapped) {

        //first determine +- of vector2 to vector3 if they dont exist
        if (change == Vector3.zero) {
            Vector3 actual = die.transform.position;
            change = new Vector3(actual.x - mapped.x, actual.y, actual.z - mapped.y);
        }

        //converted vector
        return new Vector3(change.x + mapped.x, change.y, change.z + mapped.y);
    }




}
