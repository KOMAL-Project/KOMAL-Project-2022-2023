using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mechanic : MonoBehaviour
{
    public Vector2Int position;
    public abstract void CheckForActivation();

}
