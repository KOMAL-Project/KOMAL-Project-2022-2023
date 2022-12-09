using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mechanic : MonoBehaviour
{
    //board location
    public Vector2Int position;
    //ambiguous state used by all mechanics
    protected int state;
    //ambiguous type used by mechanics which have multiple of the same types
    protected int type;
    //filter connected to mechanic
    protected PipFilterController pipFilter;
    //overall checking function
    protected DieController dieControl;
    protected ManageGame gameManager;
    public abstract void CheckForActivation();

    /// <summary>
    /// Checks if the current pip on the die facing down matches the filter
    /// </summary>
    /// <returns></returns>
    public bool CheckPipFilter() {
        return pipFilter.MeetsPipRequirement(dieControl.sides[Vector3Int.down]);
    }

    /// <summary>
    /// Attaches main scripts to each mechanic script.
    /// </summary>
    /// <param name="mg"></param>
    /// <param name="dc"></param>
    /// <param name="pos"></param>
    /// <param name="pipFilterValue"></param>
    /// <param name="theType"></param>
    public void attachValues(ManageGame mg, DieController dc, Vector2Int pos, int pipFilterValue, int theType) {
        gameManager = mg;
        dieControl = dc;
        position = pos;
        pipFilter = GetComponentInChildren<PipFilterController>();
        if (pipFilter) pipFilter.pips = pipFilterValue;
        type = theType;
    }

}
