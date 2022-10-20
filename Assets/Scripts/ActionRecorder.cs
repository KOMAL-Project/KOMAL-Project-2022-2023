using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct states {
    public Action ghostRotation;
    public Vector3 actualDieLocation;
    public Vector2Int mappedDieLocation;
    public Quaternion rotation;

}

public class ActionRecorder : MonoBehaviour
{
    public GameObject die;
    public DieController dieController;
    Stack<states> stateStack;


    // Start is called before the first frame update
    void Start()
    {
        dieController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();
        die = dieController.transform.gameObject;

        stateStack = new Stack<states>();

    }

    private void Update() {
        if (Input.GetKeyUp("k")) {
            Undo();
        }
    }

    public void Record(Action diceRotation) {

        Debug.Log(dieController.sides);
        stateStack.Push(new states {
            ghostRotation = diceRotation
            ,actualDieLocation = die.transform.position
            ,mappedDieLocation = dieController.position
            ,rotation = die.transform.rotation
        });

    }

    public void Undo() {
            if (stateStack.Count == 0) {
                return;
            }         

        states undoState = stateStack.Pop();
        ReverseTurn(undoState.ghostRotation)();
        die.transform.position = undoState.actualDieLocation;
        dieController.position = undoState.mappedDieLocation;
        die.transform.rotation = undoState.rotation;
    
    }

    public Action ReverseTurn(Action input) {

        List<Action> moves = new List<Action> { dieController.MoveForward, dieController.MoveLeft, dieController.MoveBack, dieController.MoveRight };
        int index = moves.IndexOf(input);
        return moves[(index + 2) % 4];

    }

}
