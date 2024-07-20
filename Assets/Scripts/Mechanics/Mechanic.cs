using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mechanic : MonoBehaviour
{
    //location of the object on the board
    public Vector2Int position;
    //ambiguous state used by all mechanics. Details of each state is in script description. (should i move this elsewhere?)
    protected int state;
    //ambiguous type used by mechanics which have multiple of the same types
    protected int type;
    //filter connected to mechanic
    protected PipFilterController pipFilter;
    protected DieController dieControl;
    protected ManageGame gameManager;
    protected AudioSourceManager sourceManager;
    public abstract void CheckForActivation(); //overall checking function
    public abstract void SetState(int input); //overall setting state function

    /// <summary>
    /// Checks if the current pip on the die facing down matches the filter
    /// </summary>
    /// <returns></returns>
    public bool CheckPipFilter() {
        return pipFilter.MeetsPipRequirement(dieControl.sides[Vector3Int.down]);
    }

    /// <summary>
    /// Attaches main scripts to each mechanic script. Used in GenerateLevel, when mechanics are created.
    /// </summary>
    /// <param name="mg"></param>
    /// <param name="dc"></param>
    /// <param name="pos"></param>
    /// <param name="pipFilterValue"></param>
    /// <param name="theType"></param>
    public void attachValues(ManageGame mg, DieController dc, AudioSourceManager so, Vector2Int pos, int pipFilterValue, int theType) {
        gameManager = mg;
        dieControl = dc;
        sourceManager = AudioSourceManager.Instance;
        position = pos;
        pipFilter = GetComponentInChildren<PipFilterController>();
        if (pipFilter) pipFilter.pips = pipFilterValue;
        type = theType;
    }

    /// <summary>
    /// Returns the state of the mechanic. Check the script summary to find what each state means.
    /// </summary>
    /// <returns></returns>
    public int getState() {
        return state;
    }

}
