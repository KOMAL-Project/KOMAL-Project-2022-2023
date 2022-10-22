using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//struct that contains all the necessary states info needed to revert move
public struct states {
    public Action ghostRotation;
    public Vector2Int mappedDieLocation;
    public Quaternion rotation;

}

public class ActionRecorder : MonoBehaviour
{
    public GameObject die;
    public DieController dieController;
    Stack<states> stateStack;
    private Vector3 change = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        dieController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        die = dieController.transform.gameObject;

        stateStack = new Stack<states>();

    }

    private void Update() {
        //could be added somewhere else
        if (Input.GetKeyUp("k")) {
            Undo();
        }
    }

    //stores the last state and puts it in the stack
    public void Record() {

        stateStack.Push(new states {
            ghostRotation = dieController.lastAction
            ,mappedDieLocation = dieController.position
            ,rotation = die.transform.rotation
        });

    }

    public void Undo() {

        if (stateStack.Count == 0) {
            return;
        }         

        //gets the state that it is returning to
        states undoState = stateStack.Pop();

        // does stuff with it
        ReverseTurn(undoState.ghostRotation)();
        dieController.position = undoState.mappedDieLocation;
        die.transform.position = MapToActualPosition(undoState.mappedDieLocation);
        die.transform.rotation = undoState.rotation;
    
    }

    //takes a turn direction (ghosts) and does the opposite
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
