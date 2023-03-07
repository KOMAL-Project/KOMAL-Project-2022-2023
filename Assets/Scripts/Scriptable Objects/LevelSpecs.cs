using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CustomLevelSpecs", order = 2)]
public class LevelSpecs : ScriptableObject
{
    [Tooltip("how zoomed out the camera is in isometric and overhead modes")]
    public float camSizeIso = 4, camSizeOverhead = 8;
    [Tooltip("The ID of level we are applying this to. follows 'chapter-level'. Add 'b' to the end if it's a bonus level.")]
    public string levelID; 
}
