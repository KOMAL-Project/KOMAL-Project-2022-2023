using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color Palette", menuName = "ScriptableObjects/ColorPalette", order = 1)]
public class ColorPalette : ScriptableObject
{
    public Color backgroundColor;
    public Material foam;
}
